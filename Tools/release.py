#!/usr/bin/env python3

import os
import shutil
import sys
import subprocess
import traceback
import hashlib

moduleName = "YetAnotherPartyOrganiser"

###############################################################################
# http://akiscode.com/articles/sha-1directoryhash.shtml
# Copyright (c) 2009 Stephen Akiki
# MIT License (Means you can do whatever you want with this)
#  See http://www.opensource.org/licenses/mit-license.php
# Error Codes:
#   -1 -> Directory does not exist
#   -2 -> General error (see stack traceback)
def  get_directory_hash(directory):
    directory_hash = hashlib.sha1()
    if not os.path.exists (directory):
        return -1

    try:
        for root, dirs, files in os.walk(directory):
            for names in files:
                path = os.path.join(root, names)
                try:
                    f = open(path, 'rb')
                except:
                    # You can't open the file for some reason
                    f.close()
                    continue

                while 1:
                    # Read file in as little chunks
                    buf = f.read(4096)
                    if not buf: break
                    new = hashlib.sha1(buf)
                    directory_hash.update(new.digest())
                f.close()

    except:
        # Print the stack traceback
        traceback.print_exc()
        return -2

    return directory_hash.hexdigest()

def Fract_Sec(s):
    temp = float()
    temp = float(s) / (60*60*24)
    d = int(temp)
    temp = (temp - d) * 24
    h = int(temp)
    temp = (temp - h) * 60
    m = int(temp)
    temp = (temp - m) * 60
    sec = temp
    return d,h,m,sec
    #endef Fract_Sec

# Copyright (c) André Burgaud
# http://www.burgaud.com/bring-colors-to-the-windows-console-with-python/
if sys.platform == "win32":
    from ctypes import windll, Structure, c_short, c_ushort, byref

    SHORT = c_short
    WORD = c_ushort

    class COORD(Structure):
      """struct in wincon.h."""
      _fields_ = [
        ("X", SHORT),
        ("Y", SHORT)]

    class SMALL_RECT(Structure):
      """struct in wincon.h."""
      _fields_ = [
        ("Left", SHORT),
        ("Top", SHORT),
        ("Right", SHORT),
        ("Bottom", SHORT)]

    class CONSOLE_SCREEN_BUFFER_INFO(Structure):
      """struct in wincon.h."""
      _fields_ = [
        ("dwSize", COORD),
        ("dwCursorPosition", COORD),
        ("wAttributes", WORD),
        ("srWindow", SMALL_RECT),
        ("dwMaximumWindowSize", COORD)]

    # winbase.h
    STD_INPUT_HANDLE = -10
    STD_OUTPUT_HANDLE = -11
    STD_ERROR_HANDLE = -12

    # wincon.h
    FOREGROUND_BLACK     = 0x0000
    FOREGROUND_BLUE      = 0x0001
    FOREGROUND_GREEN     = 0x0002
    FOREGROUND_CYAN      = 0x0003
    FOREGROUND_RED       = 0x0004
    FOREGROUND_MAGENTA   = 0x0005
    FOREGROUND_YELLOW    = 0x0006
    FOREGROUND_GREY      = 0x0007
    FOREGROUND_INTENSITY = 0x0008 # foreground color is intensified.

    BACKGROUND_BLACK     = 0x0000
    BACKGROUND_BLUE      = 0x0010
    BACKGROUND_GREEN     = 0x0020
    BACKGROUND_CYAN      = 0x0030
    BACKGROUND_RED       = 0x0040
    BACKGROUND_MAGENTA   = 0x0050
    BACKGROUND_YELLOW    = 0x0060
    BACKGROUND_GREY      = 0x0070
    BACKGROUND_INTENSITY = 0x0080 # background color is intensified.

    stdout_handle = windll.kernel32.GetStdHandle(STD_OUTPUT_HANDLE)
    SetConsoleTextAttribute = windll.kernel32.SetConsoleTextAttribute
    GetConsoleScreenBufferInfo = windll.kernel32.GetConsoleScreenBufferInfo

    def get_text_attr():
      """Returns the character attributes (colors) of the console screen
      buffer."""
      csbi = CONSOLE_SCREEN_BUFFER_INFO()
      GetConsoleScreenBufferInfo(stdout_handle, byref(csbi))
      return csbi.wAttributes

    def set_text_attr(color):
      """Sets the character attributes (colors) of the console screen
      buffer. Color is a combination of foreground and background color,
      foreground and background intensity."""
      SetConsoleTextAttribute(stdout_handle, color)
###############################################################################

def color(color):
    """Set the color. Works on Win32 and normal terminals."""
    if sys.platform == "win32":
        if color == "green":
            set_text_attr(FOREGROUND_GREEN | get_text_attr() & 0x0070 | FOREGROUND_INTENSITY)
        elif color == "yellow":
            set_text_attr(FOREGROUND_YELLOW | get_text_attr() & 0x0070 | FOREGROUND_INTENSITY)
        elif color == "red":
            set_text_attr(FOREGROUND_RED | get_text_attr() & 0x0070 | FOREGROUND_INTENSITY)
        elif color == "blue":
            set_text_attr(FOREGROUND_BLUE | get_text_attr() & 0x0070 | FOREGROUND_INTENSITY)
        elif color == "reset":
            set_text_attr(FOREGROUND_GREY | get_text_attr() & 0x0070)
        elif color == "grey":
            set_text_attr(FOREGROUND_GREY | get_text_attr() & 0x0070)
    else :
        if color == "green":
            sys.stdout.write('\033[92m')
        elif color == "red":
            sys.stdout.write('\033[91m')
        elif color == "blue":
            sys.stdout.write('\033[94m')
        elif color == "reset":
            sys.stdout.write('\033[0m')

def print_error(msg):
    color("red")
    print ("ERROR: {}".format(msg))
    color("reset")

def print_green(msg):
    color("green")
    print(msg)
    color("reset")

def print_blue(msg):
    color("blue")
    print(msg)
    color("reset")

def print_yellow(msg):
    color("yellow")
    print(msg)
    color("reset")

def compile_extension(extensionPath, outputPath):
    originalDir = os.getcwd()

    try:
        print_blue("\nCompiling extension in {}".format(extensionPath))
        os.chdir(extensionPath)
        print()
        ret = subprocess.call(["msbuild.exe", "{}.sln".format(moduleName), "/m", "/p:Configuration=Release", "/p:OutputPath={}".format(outputPath)])
        if ret == 1:
            print_error("\nFailed to compile extension")
            return 1
    except:
        print_error("Failed to compile extension")
        raise
    finally:
        os.chdir(originalDir)


def main(argv):
    print("""
##################################
# Bannerlod Module Release Build #
##################################
""")

    root = os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
    releasePath = os.path.join(root, "Release")
    extensionPath = os.path.join(root, "Extension")

    os.chdir(root)

    # Clean up old release files
    if (os.path.exists(releasePath)):
        shutil.rmtree(releasePath, True)

    if (os.path.exists(os.path.join(root, "{}.zip".format(moduleName)))):
        os.remove(os.path.join(root, "{}.zip".format(moduleName)))

    # Build extension
    ret = compile_extension(extensionPath, os.path.join(releasePath, "Modules", moduleName))
    if ret == 1:
        raise Exception()

    # Make release zip
    shutil.make_archive("{}".format(moduleName), "zip", releasePath)


if __name__ == "__main__":
    sys.exit(main(sys.argv))
