String r = "";
byte requested_field = 0;
int baud = 9600;
byte self_test = 2;

int port_numbers[8][3] = {
  { 22, 23, 24 },
  { 26, 27, 28 },
  { 30, 31, 32 },
  { 34, 35, 36 },
  { 38, 39, 40 },
  { 42, 43, 44 },
  { 46, 47, 48 },
  { 50, 51, 52 }
};

byte selected_multiplexer = 0;
byte selected_field = 0;

String errors = "";


//INIT

void setup() {

  errors = "EP:16,";
  // Startup Config
  Serial.begin(baud);
  //Serial.println(port);
  int a = 0;
  for (int i = 0; i < 3; i++) {
    delay(200);
    pinMode(port_numbers[a][i], OUTPUT);
    //Serial.println(port_numbers[a][i]);
    if (a == 7 && i == 2) i = 3;
    if (i == 2) {
      a++;
      i = -1;
    }
    pinMode(A0, INPUT);
    pinMode(A1, INPUT);
    pinMode(A2, INPUT);
    pinMode(A3, INPUT);
    pinMode(A4, INPUT);
    pinMode(A5, INPUT);
    pinMode(A6, INPUT);
    pinMode(A7, INPUT);
  }



  //Self Test

  if (self_test == 2) {

    //Test Multiplexers
    int out1;
    int out2;
    for (int i = 0; i < 8; i++) {

      //Serial.println("Bauer auf Feld 1 Reihe " + String(i) + " stellen");
      // EM = ERROR MULTIPLEXER
      int out1 = analogRead(55 + i);
      digitalWrite(port_numbers[i][0], HIGH);
      delay(100);
      int out2 = analogRead(55 + i);
      //Serial.println(out2);
      if (out2 > 800 && out1 > 800) {
        //Serial.println("EM: " + String(i));
        errors += "EM:" + String(i) + ",";
      } else if (out2 == 0 && out1 == 0) {
        //Serial.println("EM: " + String(i));
        errors += "EM:" + String(i) + ",";
      } else if (out2 == 1023 && out1 == 1023) {
        //Serial.println("EM: " + String(i));
        errors += "EM:" + String(i) + ",";
      } else if (out2 < 300 && out1 < 300) {
        //Serial.println("EM: " + String(i));
        errors += "EM:" + String(i) + ",";
      }
      digitalWrite(port_numbers[i][0], LOW);
    }

    //Test Felder
    // ERROR POSITION
    for (short i = 0; i < 77; i++) {
      short last_i = getLastDigit(i);
      if (last_i > 7) {
        short factor = 10 - last_i;
        i += factor;
      };
      int resp = qget(i, false);
      delay(50);
      int resp2 = qget(i, false);
      if (resp > 800 && resp2 > 800) {
        //Serial.println("EP: " + String(i));
        errors += "EP:" + String(i) + ",";
      } else if (resp == 0 && resp2 == 0) {
        //Serial.println("EP: " + String(i));
        errors += "EP:" + String(i) + ",";
      } else if (resp == 1023 && resp2 == 1023) {
        //Serial.println("EP: " + String(i));
        errors += "EP:" + String(i) + ",";
      } else if (resp < 300 && resp2 < 300) {
        //Serial.println("EP: " + String(i));
        errors += "EP:" + String(i) + ",";
      }
    }
    Serial.println("READY");
  }
}


short getLastDigit(short number) {
  for (; number > 9; number -= 10) {}
  return number;
}

bool convertBool(char number) {
  if (number == '1') return true;
  else return false;
}

void loop() {

  if (Serial.available() > 0) {
    // read the incoming byte:
    r = Serial.readStringUntil('\n');

    if (r.startsWith("QGET") || r.startsWith("qget") || r.startsWith("QGet")) {
      Serial.println("RB:" + String(requested_field));
      Serial.println(qget(r.substring(4).toInt(), false));
    }

    if (r.startsWith("QSTREAM") || r.startsWith("qstream") || r.startsWith("QStream")) {
      Serial.println("RB:" + String(requested_field));
      Serial.println(qget(r.substring(7).toInt(), true));
    }

    if (r.startsWith("QRANGE") || r.startsWith("qrange") || r.startsWith("QRange")) {
      Serial.println("RB:" + String(requested_field));
      qgetRange(r.substring(6).toInt());
    }

    if (r == "QDOUBLE" || r == "qdouble" || r == "QDouble") {
      Serial.println("RB:" + String(requested_field));
      qget(0, false);
      qget(10, false);
      byte iterator = 0;
      while (loop && String("X") != Serial.readStringUntil('\n') || iterator == 255) {
        Serial.println(String(analogRead(A0)) + "," + String(analogRead(A1)));
        iterator++;
      }
    }

    if (r == "QALL" || r == "QAll" || r == "qall") {
      Serial.println("RB:" + String(requested_field));
      qgetall(false);
    }

    if (r == "INFO" || r == "Info" || r == "info") {
      Serial.println("SYSTEM INFO:");
      Serial.println("BAUD: " + String(baud));
      Serial.println("JuFo Schach Running");
      Serial.println("Self Test Result:");
      Serial.println(errors);
    }

    if (r == "RESULT" || r == "Result" || r == "result") {
      Serial.println(errors);
    }

    if (r == "TEST" || r == "Test" || r == "test") {
      setup();
    }
  }
}

int qget(int request, bool loop) {
  int sel = request / 10;
  int hintere = request;
  for (; hintere > 9; hintere -= 10) {}
  selected_field = hintere;
  String assigned = String(selected_field, BIN);
  while (assigned.length() < 3) assigned = "0" + assigned;
  if (assigned.length() > 3) {
    Serial.println("EQ:NA");
  } else {
    bool strg_a = convertBool(assigned.charAt(0));
    bool strg_b = convertBool(assigned.charAt(1));
    bool strg_c = convertBool(assigned.charAt(2));

    digitalWrite(port_numbers[sel][0], strg_a ? HIGH : LOW);
    digitalWrite(port_numbers[sel][1], strg_b ? HIGH : LOW);
    digitalWrite(port_numbers[sel][2], strg_c ? HIGH : LOW);
    int resp = analogRead(54 + sel);

    while(loop && String("X") != Serial.readStringUntil('\n')) {
      Serial.println(analogRead(54 + sel));
      Serial.println(analogRead(54 + sel));
    }

    return resp;
  }
}

void qgetRange(int request) {
    for(int i = 7; i >= 0; i--) {
      String assigned = String(i, BIN);
      while (assigned.length() < 3) assigned = "0" + assigned;
      digitalWrite(port_numbers[request][0], convertBool(assigned.charAt(0)) ? HIGH : LOW);
      digitalWrite(port_numbers[request][1], convertBool(assigned.charAt(1)) ? HIGH : LOW);
      digitalWrite(port_numbers[request][2], convertBool(assigned.charAt(2)) ? HIGH : LOW);
      Serial.println(analogRead(54 + request));
    }
}

void qgetall(bool loop) {
  bool tloop = true;
  while (tloop && String("X") != Serial.readStringUntil('\n')) {
    for(int i = 0; i < 8; i++) {
      qgetRange(i*10);
    }
    tloop = loop;
  }
}
