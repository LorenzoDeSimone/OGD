import os
from PIL import Image
import os

for root, subdirs, files in os.walk("."):
    for file in files:
        if os.path.splitext(file)[1].lower() in ('.png'):
             print('working...')
             img = Image.open(os.path.join(root, file))
             new_img = img.transpose(Image.FLIP_LEFT_RIGHT) 
             new_img.save(os.path.join(root, file))