#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <LiquidCrystal_I2C.h>
#include <DHT.h>
#include <ArduinoJson.h>
#include <ESP32Servo.h>

/* PIN CONFIG */
#define BTN_SCAN 13
#define BTN_CONFIRM 12
#define BTN_EMERGENCY 15

#define LED_OK 26
#define LED_ERR 27
#define LED_STATUS 33
#define BUZZER 25
#define SERVO_PIN 14

#define DHTPIN 4
#define DHTTYPE DHT22

/* OBJECTS */
LiquidCrystal_I2C lcd(0x27, 16, 2);
DHT dht(DHTPIN, DHTTYPE);
Servo servoMotor;  

/* WIFI */
const char* ssid = "Wokwi-GUEST";
const char* password = "";

/* SERVER */
String baseUrl = "https://safedoseapi.whitesky-2b556a71.northeurope.azurecontainerapps.io/api/iot";
String scanUrl = baseUrl + "/scan";
String statusUrl = baseUrl + "/status";
String logUrl = baseUrl + "/log";
String dispenseUrl = baseUrl + "/dispense";
String inventoryUrl = baseUrl + "/inventory";

/* DEVICE DATA */
String deviceId = "ESP32-01";
long workplaceId = 1;
int currentMedicineId = 1;
int currentUserId = 1;
int inventoryCount = 10;

/* STATE MACHINE */
enum DeviceState {
  IDLE,
  WAIT_CONFIRM,
  DISPENSING,
  ERROR_STATE,
  MAINTENANCE,
  LOW_INVENTORY
};

DeviceState state = IDLE;

/* TIMERS И DEBOUNCING */
unsigned long confirmStart = 0;
unsigned long lastStatusUpdate = 0;
unsigned long lastInventoryCheck = 0;
unsigned long lastButtonPress = 0;
unsigned long lastLEDUpdate = 0;

const unsigned long CONFIRM_TIMEOUT = 30000;
const unsigned long STATUS_INTERVAL = 30000;
const unsigned long INVENTORY_INTERVAL = 60000;
const unsigned long BUTTON_DEBOUNCE = 300; 
const unsigned long LED_BLINK_INTERVAL = 500; 

/* BUTTON STATE TRACKING */
bool lastScanState = HIGH;
bool lastConfirmState = HIGH;
bool lastEmergencyState = HIGH;
bool ledBlinkState = false;

/* PRESCRIPTION DATA */
struct PrescriptionData {
  long prescriptionId;
  long patientId;
  long doctorId;
  long medicationId;
  int dosage;
  String patientName;
  String medicationName;
  bool isValid;
};

PrescriptionData currentPrescription;

/* SERVO FUNCTIONS */
void setupServo() {
  servoMotor.attach(SERVO_PIN);
  servoMotor.write(0); 
  delay(500);
}

void setServoAngle(int angle) {
  servoMotor.write(angle);
  delay(15); 
}

/* BUTTON FUNCTIONS */
bool readButtonWithDebounce(int pin, bool &lastState) {
  bool currentState = digitalRead(pin);
  if (currentState != lastState && millis() - lastButtonPress > BUTTON_DEBOUNCE) {
    lastState = currentState;
    lastButtonPress = millis();
    return (currentState == LOW); 
  }
  return false;
}

/* LED FUNCTIONS */
void updateStatusLED() {
  if (millis() - lastLEDUpdate < LED_BLINK_INTERVAL) return;
  
  lastLEDUpdate = millis();
  
  switch (state) {
    case IDLE:
      digitalWrite(LED_STATUS, HIGH);
      break;
      
    case WAIT_CONFIRM:
      ledBlinkState = !ledBlinkState;
      digitalWrite(LED_STATUS, ledBlinkState ? HIGH : LOW);
      break;
      
    case DISPENSING:
      digitalWrite(LED_STATUS, HIGH);
      digitalWrite(LED_OK, HIGH);
      break;
      
    case ERROR_STATE:
    case LOW_INVENTORY:
      ledBlinkState = !ledBlinkState;
      digitalWrite(LED_STATUS, ledBlinkState ? HIGH : LOW);
      if (state == ERROR_STATE) {
        digitalWrite(LED_ERR, ledBlinkState ? HIGH : LOW);
      }
      break;
      
    default:
      digitalWrite(LED_STATUS, LOW);
      break;
  }
}

/* FUNCTIONS */
void processScan();
void dispense();
void errorSignal();
void sendStatus(String status);
void sendLog(String description);
void checkInventory();
void handleEmergency();
void displayInfo();

/* SETUP */
void setup() {
  Serial.begin(115200);
  Serial.println("Starting Smart Medicine Dispenser...");

  pinMode(BTN_SCAN, INPUT_PULLUP);
  pinMode(BTN_CONFIRM, INPUT_PULLUP);
  pinMode(BTN_EMERGENCY, INPUT_PULLUP);
  pinMode(LED_OK, OUTPUT);
  pinMode(LED_ERR, OUTPUT);
  pinMode(LED_STATUS, OUTPUT);
  pinMode(BUZZER, OUTPUT);

  digitalWrite(LED_OK, LOW);
  digitalWrite(LED_ERR, LOW);
  digitalWrite(LED_STATUS, LOW);

  setupServo();
  
  lcd.init();
  lcd.backlight();
  lcd.print("Initializing...");
  delay(1000);
  
  dht.begin();

  lcd.clear();
  lcd.print("WiFi Connecting");
  WiFi.begin(ssid, password);

  int wifiAttempts = 0;
  while (WiFi.status() != WL_CONNECTED && wifiAttempts < 20) {
    delay(500);
    Serial.print(".");
    lcd.print(".");
    wifiAttempts++;
  }

  if (WiFi.status() == WL_CONNECTED) {
    Serial.println("\nWiFi Connected!");
    Serial.print("IP: ");
    Serial.println(WiFi.localIP());
    
    lcd.clear();
    lcd.print("Device Ready");
    digitalWrite(LED_STATUS, HIGH);
    
    sendLog("Device started successfully");
    sendStatus("online");
  } else {
    Serial.println("\nWiFi Failed!");
    lcd.clear();
    lcd.print("WiFi Failed");
    state = ERROR_STATE;
  }
  
  currentPrescription.isValid = false;
  
  Serial.println("Setup completed");
}

/* MAIN LOOP */
void loop() {
  updateStatusLED();
  
  if (readButtonWithDebounce(BTN_EMERGENCY, lastEmergencyState)) {
    handleEmergency();
  }

  if (millis() - lastStatusUpdate > STATUS_INTERVAL) {
    if (state != ERROR_STATE) {
      sendStatus("operational");
    }
    lastStatusUpdate = millis();
  }

  if (millis() - lastInventoryCheck > INVENTORY_INTERVAL) {
    checkInventory();
    lastInventoryCheck = millis();
  }

  switch (state) {
    case IDLE:
      if (readButtonWithDebounce(BTN_SCAN, lastScanState)) {
        processScan();
      }
      break;

    case WAIT_CONFIRM:
      displayInfo();
      if (readButtonWithDebounce(BTN_CONFIRM, lastConfirmState)) {
        dispense();
      }
      
      if (millis() - confirmStart > CONFIRM_TIMEOUT) {
        lcd.clear();
        lcd.print("Session Timeout");
        Serial.println("Session timeout occurred");
        errorSignal();
        sendLog("Confirmation timeout occurred");
        currentPrescription.isValid = false;
        state = IDLE;
        delay(2000);
        lcd.clear();
        lcd.print("Device Ready");
      }
      break;

    case LOW_INVENTORY:
      lcd.setCursor(0, 0);
      lcd.print("LOW INVENTORY");
      lcd.setCursor(0, 1);
      lcd.print("Count: " + String(inventoryCount));
      if (readButtonWithDebounce(BTN_SCAN, lastScanState)) {
        state = IDLE;
        lcd.clear();
        lcd.print("Device Ready");
      }
      break;

    case ERROR_STATE:
      lcd.setCursor(0, 0);
      lcd.print("ERROR STATE");
      lcd.setCursor(0, 1);
      lcd.print("Press SCAN");
      if (readButtonWithDebounce(BTN_SCAN, lastScanState)) {
        state = IDLE;
        digitalWrite(LED_ERR, LOW);
        lcd.clear();
        lcd.print("Device Ready");
        sendLog("Device recovered from error state");
      }
      break;
  }
  
  delay(10);
}

/* SCAN PROCESS */
void processScan() {
  lcd.clear();
  lcd.print("Scanning...");
  Serial.println("Scan initiated");
  sendLog("Scan initiated");

  HTTPClient http;
  http.begin(scanUrl);
  http.addHeader("Content-Type", "application/json");
  http.setTimeout(10000); 

  DynamicJsonDocument doc(1024);
  doc["deviceId"] = deviceId;
  doc["medicineId"] = currentMedicineId;
  doc["userId"] = currentUserId;
  doc["workplaceId"] = workplaceId;

  String payload;
  serializeJson(doc, payload);
  
  Serial.println("Sending request: " + payload);

  int code = http.POST(payload);
  String response = http.getString();
  http.end();

  Serial.println("Response code: " + String(code));
  Serial.println("Response: " + response);

  if (code == 200) {
    DynamicJsonDocument responseDoc(2048);
    DeserializationError error = deserializeJson(responseDoc, response);
    
    if (error) {
      Serial.println("JSON parsing error");
      lcd.clear();
      lcd.print("Data Error");
      errorSignal();
      state = ERROR_STATE;
      return;
    }
    
    if (responseDoc["allowed"] == true) {
      currentPrescription.prescriptionId = responseDoc["prescriptionId"] | 0;
      currentPrescription.patientId = responseDoc["patientId"] | 0;
      currentPrescription.doctorId = responseDoc["doctorId"] | 0;
      currentPrescription.medicationId = responseDoc["medicationId"] | 0;
      currentPrescription.dosage = responseDoc["dosage"] | 1;
      currentPrescription.patientName = responseDoc["patientName"] | "Unknown Patient";
      currentPrescription.medicationName = responseDoc["medicationName"] | "Unknown Medicine";
      currentPrescription.isValid = true;

      lcd.clear();
      lcd.print("Authorized");
      Serial.println("Authorization successful for: " + currentPrescription.patientName);
      
      state = WAIT_CONFIRM;
      confirmStart = millis();
      
      sendLog("Authorization successful for patient: " + currentPrescription.patientName);
    } else {
      lcd.clear();
      lcd.print("Access Denied");
      String reason = responseDoc["reason"] | "Unknown reason";
      Serial.println("Access denied: " + reason);
      errorSignal();
      sendStatus("denied");
      sendLog("Authorization denied - " + reason);
      state = IDLE;
      delay(3000);
      lcd.clear();
      lcd.print("Device Ready");
    }
  } else {
    lcd.clear();
    lcd.print("Network Error");
    Serial.println("Network error - Code: " + String(code));
    errorSignal();
    sendLog("Network error during scan - Code: " + String(code));
    state = ERROR_STATE;
  }
}

/* DISPENSE PROCESS */
void dispense() {
  if (!currentPrescription.isValid) {
    Serial.println("Invalid prescription data");
    errorSignal();
    return;
  }

  state = DISPENSING;
  lcd.clear();
  lcd.print("Dispensing...");
  Serial.println("Dispensing started for prescription ID: " + String(currentPrescription.prescriptionId));
  sendLog("Dispensing started for prescription ID: " + String(currentPrescription.prescriptionId));

  digitalWrite(LED_OK, HIGH);
  digitalWrite(LED_STATUS, HIGH);

  Serial.println("Moving servo...");
  for (int i = 0; i <= 90; i += 5) {
    setServoAngle(i);
    delay(30);
  }
  delay(1000);
  for (int i = 90; i >= 0; i -= 5) {
    setServoAngle(i);
    delay(30);
  }

  digitalWrite(LED_OK, LOW);
  inventoryCount--;

  HTTPClient http;
  http.begin(dispenseUrl);
  http.addHeader("Content-Type", "application/json");
  http.setTimeout(10000);

  float temp = dht.readTemperature();
  if (isnan(temp)) temp = 25.0; 

  DynamicJsonDocument doc(1024);
  doc["deviceId"] = deviceId;
  doc["prescriptionId"] = currentPrescription.prescriptionId;
  doc["patientId"] = currentPrescription.patientId;
  doc["doctorId"] = currentPrescription.doctorId;
  doc["medicationId"] = currentPrescription.medicationId;
  doc["quantityDispensed"] = currentPrescription.dosage;
  doc["temperature"] = temp;
  doc["inventoryCount"] = inventoryCount;

  String payload;
  serializeJson(doc, payload);

  int responseCode = http.POST(payload);
  http.end();
  
  Serial.println("Dispense event sent, code: " + String(responseCode));

  lcd.clear();
  lcd.print("Dispensed!");
  lcd.setCursor(0, 1);
  String medName = currentPrescription.medicationName;
  if (medName.length() > 16) {
    medName = medName.substring(0, 16);
  }
  lcd.print(medName);
  
  Serial.println("Medication dispensed successfully");
  sendLog("Medication dispensed successfully");
  sendStatus("dispensed");
  
  delay(3000);
  currentPrescription.isValid = false;
  state = IDLE;
  
  lcd.clear();
  lcd.print("Device Ready");
}

/* EMERGENCY HANDLER */
void handleEmergency() {
  Serial.println("EMERGENCY BUTTON PRESSED!");
  sendLog("Emergency button pressed");
  sendStatus("emergency");
  
  lcd.clear();
  lcd.print("EMERGENCY!");
  lcd.setCursor(0, 1);
  lcd.print("Help Called");
  
  digitalWrite(LED_ERR, HIGH);
  for (int i = 0; i < 5; i++) {
    tone(BUZZER, 2000);
    delay(200);
    noTone(BUZZER);
    delay(200);
  }
  digitalWrite(LED_ERR, LOW);
  
  delay(2000);
  if (state == IDLE) {
    lcd.clear();
    lcd.print("Device Ready");
  }
}

/* DISPLAY INFO */
void displayInfo() {
  static unsigned long lastDisplayUpdate = 0;
  if (millis() - lastDisplayUpdate < 1000) return; 
  
  lastDisplayUpdate = millis();
  
  if (currentPrescription.isValid) {
    lcd.setCursor(0, 0);
    String patientName = currentPrescription.patientName;
    if (patientName.length() > 16) {
      patientName = patientName.substring(0, 16);
    }
    lcd.print(patientName);
    
    lcd.setCursor(0, 1);
    lcd.print("Press CONFIRM   "); 
    
    unsigned long remaining = (CONFIRM_TIMEOUT - (millis() - confirmStart)) / 1000;
    if (remaining < 10) {
      lcd.setCursor(14, 1);
      lcd.print(String(remaining));
    }
  }
}

/* INVENTORY CHECK */
void checkInventory() {
  if (inventoryCount <= 5 && state != LOW_INVENTORY) {
    state = LOW_INVENTORY;
    Serial.println("Low inventory warning - Count: " + String(inventoryCount));
    sendLog("Low inventory warning - Count: " + String(inventoryCount));
    sendStatus("low_inventory");
  }
  
  HTTPClient http;
  http.begin(inventoryUrl);
  http.addHeader("Content-Type", "application/json");
  http.setTimeout(5000);

  DynamicJsonDocument doc(512);
  doc["deviceId"] = deviceId;
  doc["medicationId"] = currentMedicineId;
  doc["currentCount"] = inventoryCount;

  String payload;
  serializeJson(doc, payload);

  http.POST(payload);
  http.end();
}

/* ERROR SIGNAL */
void errorSignal() {
  Serial.println("Error signal triggered");
  digitalWrite(LED_ERR, HIGH);
  for (int i = 0; i < 3; i++) {
    tone(BUZZER, 1000);
    delay(300);
    noTone(BUZZER);
    delay(300);
  }
  digitalWrite(LED_ERR, LOW);
}

/* STATUS UPDATE */
void sendStatus(String status) {
  float temp = dht.readTemperature();
  float humidity = dht.readHumidity();
  if (isnan(temp)) temp = 25.0;
  if (isnan(humidity)) humidity = 50.0;

  HTTPClient http;
  http.begin(statusUrl);
  http.addHeader("Content-Type", "application/json");
  http.setTimeout(5000);

  DynamicJsonDocument doc(1024);
  doc["deviceId"] = deviceId;
  doc["status"] = status;
  doc["temperature"] = temp;
  doc["humidity"] = humidity;
  doc["inventoryCount"] = inventoryCount;
  doc["workplaceId"] = workplaceId;
  doc["uptime"] = millis();

  String payload;
  serializeJson(doc, payload);

  int code = http.POST(payload);
  http.end();
  
  Serial.println("Status sent: " + status + " (Code: " + String(code) + ")");
}

/* LOGGING */
void sendLog(String description) {
  HTTPClient http;
  http.begin(logUrl);
  http.addHeader("Content-Type", "application/json");
  http.setTimeout(5000);

  DynamicJsonDocument doc(512);
  doc["deviceId"] = deviceId;
  doc["description"] = description;

  String payload;
  serializeJson(doc, payload);

  http.POST(payload);
  http.end();

  Serial.println("LOG: " + description);
}