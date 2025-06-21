# Image Processing Filters in C#

## Overview

This project is a Windows Forms application that implements various image processing filters in C#. It was developed during a fintech internship to perform low-level image manipulation tasks using bitwise operations.

## Features

### Basic Operations
- Load and save images in BMP and JPG formats  
- Undo functionality for operations  

### Image Processing Filters

#### Binarization
- Static threshold binarization  
- Mean threshold binarization  

#### Image Concatenation
- Horizontal concatenation  
- Vertical concatenation  

#### Format Conversion
- Convert 24-bit color to 8-bit grayscale  
- Convert 1-bit image to 8-bit grayscale  

#### Morphological Operations
- Dilation (3x3 kernel)  
- Erosion (3x3 kernel)  

#### Boundary Processing
- Remove white boundaries from images  

#### Resizing
- Image resizing with percentage scaling  

#### Shape Detection (Milestone)
- Connected component labeling  
- Shape recognition (rectangles, squares, triangles, circles)  

## How to Run

### Prerequisites
- .NET Framework 4.7  
- Visual Studio (recommended) or .NET SDK  

### Running the Application
1. Open the solution in Visual Studio  
2. Build the solution (`Ctrl + Shift + B`)  
3. Run the application (`F5`)  

## How to Use

1. Load an image using the `File → Load` menu  
2. Apply filters from the various menu options  
3. Save the result using `File → Save`  
4. Undo operations using `Edit → Undo`  

## Technical Details

- Windows Forms for the UI  
- Low-level bitmap manipulation via `BitmapData` and `unsafe` code  
- Connected component labeling for shape detection  
- Bilinear interpolation for resizing  

