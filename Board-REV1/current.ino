#include "driver/adc.h"
#include "esp32-hal.h"

// Updated pin definitions for ESP32-S3
#define MP0 6   // Updated from original 38
#define MP1 15  // Updated from original 47
#define MP2 16  // Updated from original 48

#define MPV0 17
#define MPV1 18
#define MPV2 21

// Updated digital pins for VCC
#define Q0 5
#define Q1 6
#define Q2 7
#define Q3 8
#define Q4 9
#define Q5 10
#define Q6 17
#define Q7 18
const byte qpins[8] = { Q0, Q1, Q2, Q3, Q4, Q5, Q6, Q7 };

String r = "";
byte requested_field = 0;
int baud = 19200;
bool self_test = false;
bool rb = true;
bool interpolate = false;
byte selected_plus = 0;
byte selected_minus = 0;

String errors = "";

void setup() {
  // Enable USB CDC
  Serial.begin(baud);
  Serial.setTimeout(32);
  while (!Serial)
    ;  // Wait for USB CDC to initialize

  errors = "";
  Serial.println("SYSTEM START - ESP32-S3");

  // Clear GPIOs
  gpio_reset_pin((gpio_num_t)1);
  gpio_reset_pin((gpio_num_t)2);
  gpio_reset_pin((gpio_num_t)3);
  gpio_reset_pin((gpio_num_t)4);
  gpio_reset_pin((gpio_num_t)11);
  gpio_reset_pin((gpio_num_t)12);
  gpio_reset_pin((gpio_num_t)13);
  gpio_reset_pin((gpio_num_t)14);

  // Configure multiplexer pins
  pinMode(MP0, OUTPUT);
  pinMode(MP1, OUTPUT);
  pinMode(MP2, OUTPUT);

  // Self Test
  bool tmp = interpolate;
  interpolate = false;
  if (self_test) {
    for (int y = 0; y < 8; y++) {
      for (int x = 0; x < 8; x++) {
        String field = String(y) + String(x);
        int resp = qget(field, false);
        delay(50);
        int resp2 = qget(field, false);
        if (resp > 3000 && resp2 > 3000) {
          errors += "EP:" + field + ",";
        } else if (resp < 1000 && resp2 < 1000) {
          errors += "EP:" + field + ",";
        }
      }
    }
  }
  interpolate = tmp;

  // Initialize outputs
  digitalWrite(MP0, LOW);
  digitalWrite(MP1, LOW);
  digitalWrite(MP2, LOW);

  Serial.println("READY");
}

// Rest of your existing functions remain largely the same,
// but with updated ADC handling for ESP32-S3:

short ADCRead(uint8_t pin) {
  // ESP32-S3 has different ADC handling
  int pins[8] = { 1,2,3,4,11,12,13,14 };
  analogReadResolution(12);  // Set to 12-bit resolution
  return analogRead(pins[pin]);
}

void writeMP(byte val) {
  val &= 0b111;
  digitalWrite(MP0, val & 0b001);
  digitalWrite(MP1, val & 0b010);
  digitalWrite(MP2, val & 0b100);
}

void writeQ(byte pin) {
  for (uint8_t i = 0; i < 8; i++) {
    digitalWrite(qpins[i], (i == pin) ? HIGH : LOW);
  }
}

void writeMPV(byte val) {
  val &= 0b111;
  digitalWrite(MPV0, val & 0b001);
  digitalWrite(MPV1, val & 0b010);
  digitalWrite(MPV2, val & 0b100);
}

// Keep all your existing qget, qgetRange, qgetall functions exactly the same
// [Previous implementations of qget, qgetRange, qgetall remain unchanged]

void loop() {
  if (Serial.available() > 0) {
    r = Serial.readStringUntil('\n');
    String rLower = r;
    rLower.toLowerCase();
    if (rb) Serial.println("RB:" + r);
    if (rLower.startsWith("qget")) {
      Serial.println(qget(r.substring(4), false));
    } else if (rLower.startsWith("qstream")) {
      qget(r.substring(7), true);
    } else if (rLower.startsWith("qrange")) {
      qgetRange(r.substring(6), false);
    } else if (rLower.startsWith("qsrange")) {
      qgetRange(r.substring(7), true);
    } else if (rLower.startsWith("qall")) {
      qgetall(false);
    } else if (rLower.startsWith("qsall")) {
      qgetall(true);
    } else if (rLower.startsWith("info")) {
      Serial.println("SYSTEM INFO:\r\nBAUD: " + String(baud) + "\r\nVERSION REV1\r\nJuFo Schach\r\nSelf Test Result:");
      Serial.println(errors);
    } else if (rLower.startsWith("result")) {
      Serial.println(errors);
      Serial.flush();
    } else if (rLower.startsWith("test")) {
      setup();
    } else if (rLower.startsWith("ip")) {
      interpolate = !interpolate;
    } else if (rLower.startsWith("rb")) {
      rb = !rb;
    }
  }
}

int qget(String request, bool loop) {
  String buff = request;
  buff.trim();
  byte n = buff.charAt(0) - '0';
  byte p = buff.charAt(1) - '0';
  writeMP(n);
  //writeQ(p);
  writeMPV(p);

  while (loop && String("X") != Serial.readStringUntil('\n')) {
    Serial.println(ADCRead(n));
  }

  if (interpolate) {
    int val0 = ADCRead(n);
    int val1 = ADCRead(n);
    int val2 = ADCRead(n);

    int minVal = std::min({ val0, val1, val2 });
    int maxVal = std::max({ val0, val1, val2 });

    // Prüfe, ob die Differenz zu groß ist
    if (maxVal - minVal > 20) return -1;

    return ((val0 + val1 + val2) / 3);
  }

  return ADCRead(n);
}


void qgetRange(String request, bool loop) {
  String buff = request;
  buff.trim();
  byte n = buff.charAt(0) - '0';
  bool tloop = true;
  writeMP(n);
  //Serial.println("After MP");
  while (tloop && String("X") != Serial.readStringUntil('\n')) {
    for (int i = 7; i >= 0; i--) {
      //writeQ(i);
      writeMPV(i);

      if (interpolate) {
        int val0 = ADCRead(n);
        int val1 = ADCRead(n);
        int val2 = ADCRead(n);

        int minVal = std::min({ val0, val1, val2 });
        int maxVal = std::max({ val0, val1, val2 });

        // Prüfe, ob die Differenz zu groß ist
        if (maxVal - minVal > 20) {
          Serial.println(-1);
          return;
        }

        Serial.println((val0 + val1 + val2) / 3);
      }
      else {
        Serial.println(ADCRead(n));
      }
    }
    tloop = loop;
  }
  Serial.flush();
}

void qgetall(bool loop) {
  bool tloop = true;
  while (tloop && String("X") != Serial.readStringUntil('\n')) {
    for (int i = 0; i < 8; i++) {
      qgetRange(String(i * 10), false);
    }
    tloop = loop;
  }
}
