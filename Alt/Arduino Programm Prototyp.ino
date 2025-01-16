const float uf = 0.1666; 
int B;
int Bov;
int fi; // Bauer=1, Springer=2, LÃ¤ufer=3, Turm=4, Dame=5, KÃ¶nig=6
int fif; //Farbe der Figuren 1=WeiÃŸ 2=Schwarz
int vi[9]={0,495,515,500,500,500,500,500,500};
int gz[7]={0,47,32,23,12,5,1};
void setup() {
Serial.begin (9600);  //Die serielle Schnittstelle wird aktiviert und die Ãœbertragungsgeschwindigkeit angegeben.
 
}

void loop() {
for (int i=1;i<=1;i=i+1){
  B= floor((analogRead(i)-vi[i])*uf +0.5);
Bov=0;
fif=0;
fi=0;
if (B<0) {Bov=-B; fif=2;}; 
if (B>0) {Bov=+B; fif=1;}; Serial.print (B);
//if (a=0) {Bov=0; fif=0; fi=0;}; Serial.print (a);
/*
if (Bov >gz[1]) {fi=1;};
if (Bov >gz[2] and Bov <=gz[1]) {fi=2;};
if (Bov >gz[3] and Bov <=gz[2]) {fi=3;};
if (Bov >gz[4] and Bov <=gz[3]) {fi=4;};
if (Bov >gz[5] and Bov <=gz[4]) {fi=5;};
if (Bov >gz[6] and Bov <=gz[5]) {fi=6;}:
if (Bov = gz[0]) {fi=0; fif=0;};*/
Serial.print ("Feld Nr."); Serial.print (i); Serial.print ("   "); Serial.print (B); Serial.print ("   "); Serial.print (Bov);
Serial.print ("   "); Serial.print (fi); Serial.print ("   "); Serial.println (fif);
delay (1000);


}

}

