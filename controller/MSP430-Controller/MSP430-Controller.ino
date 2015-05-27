char lastADCX, lastADCY, buttons;

#define BUTTON0 0b00001
#define BUTTON1 0b00010
#define BUTTON2 0b00100
#define BUTTON3 0b01000
#define BUTTON4 0b10000

#define BOARD0

#ifdef BOARD0
#define BOARDCONVERT(bool) (bool)
#endif

#ifdef BOARD1
#define BOARDCONVERT(bool) (!bool)
#endif

int px, py, pb;

void setup()
{
  delay(100);
  // put your setup code here, to run once:
  Serial.begin(9600);
  #ifdef BOARD0
  pinMode(P2_0,INPUT);
  pinMode(P2_1,INPUT);
  pinMode(P2_2,INPUT);
  pinMode(P2_3,INPUT);
  pinMode(P1_5,INPUT_PULLUP);
  pinMode(P1_3,INPUT);
  pinMode(P1_4,INPUT);
  px = P1_4;
  py = P1_3;
  pb = P1_5;
  #endif
  #ifdef BOARD1
  pinMode(P2_0,INPUT_PULLUP);
  pinMode(P2_1,INPUT_PULLUP);
  pinMode(P2_2,INPUT_PULLUP);
  pinMode(P2_3,INPUT_PULLUP);
  pinMode(P1_4,INPUT_PULLUP);
  pinMode(P1_3,INPUT);
  pinMode(P1_5,INPUT);
  px = P1_5;
  py = P1_3;
  pb = P1_4;
  #endif

  delay(100);
}

char Button0()
{
   if (BOARDCONVERT(digitalRead(P2_0)))
   {
      return BUTTON0; 
   }
   else return 0;
}

char Button1()
{
   if (BOARDCONVERT(digitalRead(P2_1)))
   {
      return BUTTON1; 
   }
   else return 0;
}

char Button2()
{
   if (BOARDCONVERT(digitalRead(P2_2)))
   {
      return BUTTON2; 
   }
   else return 0;
}

char Button3()
{
   if (BOARDCONVERT(digitalRead(P2_3)))
   {
      return BUTTON3; 
   }
   else return 0;
}

char Button4()
{
   if (!digitalRead(pb))
   {
      return BUTTON4; 
   }
   else return 0;
}

struct packet
{
  char x, y, buttons, parity;
};

//CRC-8 - based on the CRC8 formulas by Dallas/Maxim
//code released under the therms of the GNU GPL 3.0 license
char CRC8(const char *data, char len) {
  char crc = 0x00;
  while (len--) {
    char extract = *data++;
    for (char tempI = 8; tempI; tempI--) {
      char sum = (crc ^ extract) & 0x01;
      crc >>= 1;
      if (sum) {
        crc ^= 0x8C;
      }
      extract >>= 1;
    }
  }
  return crc;
}

void calcParity(struct packet *p)
{
    char *raw = (char *)p;
    p->parity = CRC8(raw, 3);
}

void sendPacket(struct packet *p)
{
  calcParity(p);
  Serial.write("\n");
  Serial.write((char)4);
  Serial.write(p->x);
  Serial.write(p->y);
  Serial.write(p->buttons);
  Serial.write(p->parity);
  buttons = 0;
}

void initPacket(struct packet *p)
{
   p->x = lastADCX;
   p->y = lastADCY;
   p->buttons = buttons;
}

void poll()
{
  // poll ADC
  int xValue, yValue;
  xValue = analogRead(px) - 512;
  yValue = analogRead(py) - 512;
  
  xValue >>= 2;
  yValue >>= 2;
  
  lastADCX = (char) xValue;
  lastADCY = (char) yValue;
  
  // poll buttons
  buttons = Button0() | Button1() | Button2() | Button3() | Button4();
  
  struct packet p;
  initPacket(&p);
  sendPacket(&p);
}

void loop()
{
  poll();
}


