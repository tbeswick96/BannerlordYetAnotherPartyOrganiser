#!/usr/bin/env python3

import os
import shutil
import sys

zip_name = "YetAnotherPartyOrganiser"

def main(argv):
    print("""
##################################
# Bannerlod Module Release Build #
##################################
""")

    root = os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
    modules = os.path.join(root, "Modules")
    release = os.path.join(root, "Release")

    os.chdir(root)

    if (os.path.exists(os.path.join(root, "{}.zip".format(zip_name)))):
        os.remove(os.path.join(root, "{}.zip".format(zip_name)))
        
    if (os.path.exists(release)):
        shutil.rmtree(release, True)
    
    shutil.copytree(modules, os.path.join(release, "Modules"))
    shutil.make_archive("{}".format(zip_name), "zip", release)
    shutil.rmtree(release, True)

if __name__ == "__main__":
    sys.exit(main(sys.argv))
