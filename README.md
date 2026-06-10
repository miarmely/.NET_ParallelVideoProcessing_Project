# Multi-Threaded Video Processing Framework with OpenCV and .NET

A real-time video processing application developed with C#, .NET, OpenCvSharp, and multithreading techniques. The project captures live video from a webcam, processes frames concurrently using a Producer-Consumer architecture, and performs face detection using OpenCV's Haar Cascade classifier. The system focuses on low-latency processing, efficient resource management, and scalable concurrent design.

## Features

* Real-time webcam video capture
* Haar Cascade face detection
* Producer-Consumer architecture
* Thread-safe frame processing
* BlockingCollection-based bounded buffer
* Drop Oldest Frame strategy for low-latency processing
* FPS monitoring
* Memory-efficient OpenCV resource management
* Multi-threaded processing pipeline
* Real-time video visualization

## Technologies

* C#
* .NET
* OpenCV
* OpenCvSharp
* Task Parallel Library (TPL)
* BlockingCollection
* Haar Cascade Classifier

## Architecture

The application is designed around the Producer-Consumer pattern to separate frame acquisition from frame processing.

```text
Webcam
   │
   ▼
Producer Thread
   │
   ▼
BlockingCollection Buffer
   │
   ▼
Consumer Thread
   │
   ▼
Face Detection & Processing
   │
   ▼
Display Layer
```

### Producer

The Producer continuously captures frames from the webcam and places them into a bounded BlockingCollection buffer. To prevent shared-memory issues, each frame is cloned before being inserted into the queue.

### Consumer

The Consumer retrieves frames from the buffer and applies image processing operations. Face detection is performed using the Haar Cascade classifier, and the processed frames are transferred to the display layer.

### Buffer Management

A bounded buffer is used to prevent uncontrolled memory growth. When the buffer reaches its maximum capacity, the application applies a Drop Oldest Frame strategy by removing the oldest frame before inserting a new one. This approach minimizes latency and keeps the displayed video close to real time.

## Performance Optimizations

Several optimization techniques were implemented to improve responsiveness and throughput:

* Separate Producer and Consumer threads
* Bounded buffering with BlockingCollection
* Drop Oldest Frame strategy
* Thread-safe frame sharing
* Explicit OpenCV resource disposal
* Reduced lock contention
* FPS monitoring
* Double-buffering inspired frame ownership model

These optimizations help maintain stable performance while processing live video streams.

## Face Detection

Face detection is implemented using OpenCV's Haar Cascade classifier. Each frame is analyzed to locate facial regions, and detected faces are highlighted with bounding rectangles. The detector operates in real time and integrates directly into the processing pipeline.

## Learning Outcomes

This project demonstrates practical experience with:

* Concurrent programming
* Producer-Consumer design pattern
* Thread synchronization
* Real-time video processing
* Computer vision fundamentals
* Resource ownership and memory management
* Performance optimization techniques
* OpenCV integration with .NET

## Future Improvements

Potential future enhancements include:

* Face recognition support
* Object detection models
* GPU acceleration
* Multiple consumer workers
* Video recording functionality
* Motion detection
* Distributed processing architecture

## Author

@Miarmely

**Software Engineer | Backend Developer | Database Engineer**
