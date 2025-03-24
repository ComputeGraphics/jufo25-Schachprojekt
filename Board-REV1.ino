#include "driver/adc.h"


//Digitals für Multiplexer GND
#define MP0 6   //LSB (urspr. 38 - 32 wegen out1 Gruppe)
#define MP1 15  //47
#define MP2 16  //MSB 48

//Digitals für Multiplexer 3V3
#define MPV0 17
#define MPV1 18
#define MPV2 21

//Digitals for VCC
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
int baud = 9600;
bool self_test = true;
bool rb = true;
bool interpolate = false;
byte selected_plus = 0;
byte selected_minus = 0;

String errors = "";

void setup() {
  // Startup Config
  errors = "";
  Serial.begin(baud);
  Serial.setTimeout(128);
  //Serial.println(port);
  Serial.println("SYSTEM START");
  //GPIO.enable_w1ts = (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 17) | (1 << 18);
  GPIO.enable_w1tc = (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 11) | (1 << 12) | (1 << 13) | (1 << 14);
  GPIO.enable1_w1ts.val = (1 << MP0) | (1 << MP1) | (1 << MP2);
  //Self Test
  //analogSetWidth(10);
  bool tmp = interpolate;
  interpolate = false;
  if (self_test) {
    //Test Felder
    // ERROR POSITION
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

  GPIO.out1.val = 0b00000000000000000000000000000000;
  GPIO.out = 0b00000000000000000000000000000000;

  pinMode(D8, OUTPUT);
  pinMode(D9, OUTPUT);
  pinMode(D10, OUTPUT);
  //digitalWrite(D2, HIGH);

  Serial.println("READY");
}

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
    } else if (rLower == "qall") {
      qgetall(false);
    } else if (rLower == "qsall") {
      qgetall(true);
    } else if (rLower == "info") {
      Serial.println("SYSTEM INFO:\r\nBAUD: " + String(baud) + "\r\nVERSION REV1\r\nJuFo Schach\r\nSelf Test Result:");
      Serial.println(errors);
    } else if (rLower == "result") {
      Serial.println(errors);
    } else if (rLower == "test") {
      setup();
    }
  }
}

short ADCRead(uint8_t pin) {
  adc1_config_width(ADC_WIDTH_BIT_12);
  adc1_config_channel_atten((adc1_channel_t)pin, ADC_ATTEN_DB_11);

  uint16_t adc_value = adc1_get_raw((adc1_channel_t)pin);
  return adc_value;
}


void writeMP(byte val) {
  // & - And << Bitshift
  val &= 0b111;
  //Serial.println("Prev:"+String(GPIO.out1.val,BIN));
  uint32_t mask = (1 << MP0) | (1 << MP1) | (1 << MP2);
  uint32_t shifted_value = ((val & 0b001) << MP0) |      // GPIO 38 (LSB)
                           ((val & 0b010) << MP1 - 1) |  // GPIO 47 (Middle)
                           ((val & 0b100) << MP2 - 2);   // GPIO 48 (MSB)
  GPIO.out1.val = (GPIO.out1.val & ~mask) | shifted_value;
  //Serial.println("New: "+String(GPIO.out1.val,BIN) + "," + String(GPIO.out1.val,BIN).length());
}

void writeQ(byte pin) {
  uint32_t mask = 0;
  Serial.println("Prev:" +String(GPIO.out,BIN));
  for (uint8_t i = 0; i < 8; i++) mask |= (1 << qpins[i]);
  GPIO.out_w1tc = mask;
  GPIO.out_w1ts = (1 << qpins[pin]);
  Serial.println("Next:" +String(GPIO.out,BIN));
}

void writeMPV(byte val) {
  val &= 0b111;
  uint32_t mask = (1 << MPV0) | (1 << MPV1) | (1 << MPV2);
  uint32_t shifted_value = ((val & 0b001) << MPV0) |      // GPIO 38 (LSB)
                           ((val & 0b010) << MPV1 - 1) |  // GPIO 47 (Middle)
                           ((val & 0b100) << MPV2 - 2);   // GPIO 48 (MSB)
  GPIO.out = (GPIO.out & ~mask) | shifted_value;
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
  while (tloop && String("X") != Serial.readStringUntil('\n')) {
    for (int i = 7; i >= 0; i--) {
      //writeQ(i);
      writeMPV(i);

      int val0 = ADCRead(n);
      int val1 = ADCRead(n);
      int val2 = ADCRead(n);

      if (interpolate) {
        int minVal = std::min({ val0, val1, val2 });
        int maxVal = std::max({ val0, val1, val2 });

        // Prüfe, ob die Differenz zu groß ist
        if (maxVal - minVal > 20) {
          Serial.println(-1);
          return;
        }
      }

      Serial.println((val0 + val1 + val2) / 3);
    }
    tloop = loop;
  }
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
