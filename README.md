# Cisco TMS Proxy
A cloud-hosted SOAP/XML gateway that impersonates a Cisco TMS server, enabling Cisco video endpoints to communicate with modern platforms without requiring the legacy TMS infrastructure.

## Project Overview

This service acts as a man-in-the-middle proxy between Cisco video endpoints and the C3 platform, translating SOAP/XML messages into a modern telemetry format.

### What It Does

1. **Acts as a Fake TMS Endpoint**
   - Cisco devices send SOAP/XML messages to this service instead of real TMS
   - Fully compatible with Cisco's SOAP protocol
   - Responds with proper XML success/failure messages

2. **Translates SOAP XML to C3 Format**
   - Parses incoming SOAP/XML payloads
   - Extracts device identification and telemetry data
   - Transforms data into C3's internal JSON structure
   - Forwards processed telemetry to C3 platform

## Current Status

**Phase I: Read-Only Mode - IMPLEMENTED**

This phase focuses on receiving and translating device messages:
- Receive SOAP/XML messages from Cisco devices
- Parse and validate SOAP structure
- Extract device status and identification
- Forward telemetry to C3 platform
- Store raw messages for replay capability
- Handle message bursts with async processing

**Phase II: Command Execution - FUTURE**

Planned features for bidirectional communication:
- Send commands back to devices
- Update SIP credentials
- Trigger software upgrades
- Execute device configuration changes

## Architecture

### Three-Boundary Design
 1. INGRESS LAYER
 2. PARSER LAYER
 3. FORWARDER LAYER

---

_treat people with kindness :)_
