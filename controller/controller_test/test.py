import serial
import sys
import struct


ser = serial.Serial('/dev/tty.HC-06-DevB', 9600)

state = { "x" : 0, "y" : 0, "buttons": 0} 

counter = 0
header = 0
while (1):
    if (ser.inWaiting() > 0):
        num = struct.unpack('b', ser.read(1))[0]
        if num == ord('\n') and counter == 0:
            header = 1
        else:
            if (header):
                counter = num
                header = 0
            else:
                if (counter > 0):
                    if (counter == 4):
                        state["x"] = num
                    if (counter == 3):
                        state["y"] = num
                    if (counter == 2):
                        state["buttons"] = "{0:b}".format(num)
                    counter -= 1
    try:
        print(state)
    except:
        pass
