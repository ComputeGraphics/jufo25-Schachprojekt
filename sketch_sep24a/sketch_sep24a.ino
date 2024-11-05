String r = "";
byte requested_field = 0;

int port = A15;

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

int calib_min[8][8] = {
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 }
};
int calib_max[8][8] = {
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0 }
};

byte selected_multiplexer = 0;
byte selected_field = 0;


void setup() {


  // Startup Config
  Serial.begin(9600);
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
        Serial.println("EM: " + String(i));
      }
      else if(out2 == 0 && out1 == 0) {
        Serial.println("EM: " + String(i));
      }
      else if(out2 == 1023 && out1 == 1023) {
        Serial.println("EM: " + String(i));
      }
      digitalWrite(port_numbers[i][0], LOW);
    }

    //Test Felder
    // ERROR POSITION
    for(short i = 0; i<77; i++) {
      short last_i = getLastDigit(i);
      if(last_i > 7) {
        short factor = 10 - last_i;
        i += factor;
      };
      int resp = qget(i);
      delay(50);
      int resp2 = qget(i);
      if (resp > 800 && resp2 > 800) {
        Serial.println("EP: " + String(i));
      }
      else if(resp == 0 && resp2 == 0) {
        Serial.println("EP: " + String(i));
      }
      else if(resp == 1023 && resp2 == 1023) {
        Serial.println("EP: " + String(i));
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
  // put your main code here, to run repeatedly:

  if (Serial.available() > 0) {
    int resp;
    // read the incoming byte:
    r = Serial.readStringUntil('\n');
    //Serial.println("Reading " + r);



    if (r.startsWith("GET") || r.startsWith("get") || r.startsWith("Get")) {
      requested_field = r.substring(3).toInt();
      Serial.println("RB:" + String(requested_field));

      resp = get(false, false);
    }
    

    if (r.startsWith("STREAM") || r.startsWith("Stream") || r.startsWith("stream")) {
      requested_field = r.substring(5).toInt();
      Serial.println("RB:" + String(requested_field));
        resp = get(false, true); 
    }
  }
  /*int   i = 7;

  while (i >= 0) {
    if ((nb >> i) & 1)
      printf("1");
    else
      printf("0");
    --i;
  }
  printf("\n");
*/
}


int get(bool verbose, bool stream_result) {
  selected_multiplexer = requested_field / 10;

  int hintere = requested_field;
  for (; hintere > 9; hintere -= 10) {}
  selected_field = hintere;
  if (verbose) Serial.println("Vordere Zahl: " + String(selected_multiplexer));
  if (verbose) Serial.println("Hintere Zahl: " + String(hintere));
  if (verbose) Serial.println("Binary: " + String(selected_field, BIN));
  String assigned = String(selected_field, BIN);
  while (assigned.length() < 3) assigned = "0" + assigned;
  if (verbose) Serial.println("Binary Format: " + assigned);
  if (assigned.length() > 3) {
    //ERROR INDEX OUT OF BOUNDS
    Serial.println("EG: NA");
  } else {
    bool strg_a = convertBool(assigned.charAt(0));
    bool strg_b = convertBool(assigned.charAt(1));
    bool strg_c = convertBool(assigned.charAt(2));
    if (verbose) Serial.println("Char Format: " + String(assigned.charAt(0)) + String(assigned.charAt(1)) + String(assigned.charAt(2)));

    digitalWrite(port_numbers[selected_multiplexer][0], strg_a ? HIGH : LOW);
    if (verbose) Serial.print("Setting Pin " + String(port_numbers[selected_multiplexer][0]));
    if (verbose) Serial.println(" to " + String(strg_a));

    digitalWrite(port_numbers[selected_multiplexer][1], strg_b ? HIGH : LOW);
    if (verbose) Serial.print("Setting Pin " + String(port_numbers[selected_multiplexer][1]));
    if (verbose) Serial.println(" to " + String(strg_b));

    digitalWrite(port_numbers[selected_multiplexer][2], strg_c ? HIGH : LOW);
    if (verbose) Serial.print("Setting Pin " + String(port_numbers[selected_multiplexer][2]));
    if (verbose) Serial.println(" to " + String(strg_c));
    delay(256);
    int resp = analogRead(54 + selected_multiplexer);
    //POSITION RESPONSE
    Serial.println("PR:" + String(resp));
    while(stream_result && String("X") != Serial.readStringUntil('\n')) {
      delay(256);
      resp = analogRead(54 + selected_multiplexer);
      Serial.println("PR:" + String(resp));
    }
    return resp;
  }
}

int qget(int request) {
  
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
    return resp;
  }
}
