## Cisco TMS Proxy – Project Overview
This repo is the starting point for building a cloud-hosted SOAP gateway that pretends to be a Cisco TMS server.
The goal is to let Cisco video endpoints talk to our platform (C3) without actually needing the real TMS.

### What This Project Will Eventually Do
1. Act as a fake TMS endpoint;

Cisco devices normally phone home to TMS using SOAP XML.
This service will impersonate TMS, so the devices send those messages here instead.

2. Translate SOAP XML to C3 format;

Once the device sends a SOAP payload, the service will eventually: Parse the SOAP XML; Extract info; Translate it into C3’s internal structure; Forward it to C3 as telemetry;

### Status
**Project not implemented yet.**
This README only outlines the purpose, scope, and architecture. 

- Planned phases:
  
   I. _Read-Only Mode (Current Goal)_
    This phase focuses on listening and translating. Receive SOAP messages from Cisco devices; Parse and understand the structure; Prepare to forward device status to C3

   II. _Command Execution (Future)_
    The long-term plan includes sending commands back to devices, especially:Updating SIP credentials; Triggering software upgrades; These commands will also be built as SOAP XML, matching Cisco’s expected format.
