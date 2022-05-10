# Mock Bot Technical Architecture

This document outlines the technical design for the mock chatbot component.

## Introduction

Mock Bot is a lightweight component used to simulate chatbot participation in a Decentralized Messaging Room. Its primary use cases are to:

- Enable users to send messages to DMR and read the async responses from DMR participants;
- Set up a mock participant in DMR with automatic responses mapped to specified message tokens.

Any chatbot can be integrated with DMR and CentOps as long as it supports all mandatory integrations:

| Integration                                                                                                                                                | Mandatory |
|:-----------------------------------------------------------------------------------------------------------------------------------------------------------|-----------|
| ![DMR](https://img.shields.io/badge/-DMR-blueviolet) Ask DMR for help with a specific message                                                              | M         |
| ![DMR](https://img.shields.io/badge/-DMR-blueviolet) Provide answer to a message help request                                                              | M         |
| ![CentOps](https://img.shields.io/badge/-CentOps-blue) Query list of DMR-s                                                                                 | M         |
| ![CentOps](https://img.shields.io/badge/-CentOps-blue) Receive DMR list changes                                                                            | M         |
| ![CentOps](https://img.shields.io/badge/-CentOps-blue) Notify CentOps of Chatbot status change<br/> (status of the entire chatbot, not some specific node) | M         |
| ![CentOps](https://img.shields.io/badge/-CentOps-blue) Notify CentOps of planned downtime                                                                  | O         |
| ![CentOps](https://img.shields.io/badge/-CentOps-blue) Provide health information (logs)                                                                   | O         |

## Story over DMR

// TODO: Needs further discussion, depends heavily on intended business goals

A chatbots answer may be just one step in a multistep story, e.g. `I want to renew my drivers license` might require the following steps:

1. User authentication
2. Request for an updated document image
3. Request for an updated signature sample
4. Provide delivery information
5. Pay service fees

It is therefore necessary that chat sessions can be started and continued though DMR.

//TODO: is this actually necessary or will we use actual redirect to institution? Depends on what kind of ux we are going for.

This will be managed through additional fields in help request and response payloads:

(LibraryBot -> DMR) POST /messages

```json
{
  "chatId": "3fa2cb2a-15b9-4e49-9dde-f833ffeb111c",
  "messageId": "ebcf0102-5ea3-44bc-9f26-8ad7ade3738b",
  "origin": "NationalLibrary",
  "message": "I want to renew my drivers license"
}
```

(DMR -> PoliceBot) POST /

```json
{
  "chatId": "3fa2cb2a-15b9-4e49-9dde-f833ffeb111c",
  "messageId": "3fa2cb2a-15b9-4e49-9dde-f833ffeb111c",
  "origin": "NationalLibrary",
  "message": "I want to renew my drivers license",
  "classification": [
    "Internal"
  ]
}
```

(PoliceBot -> DMR)

```json
{
  "chatId": "3fa2cb2a-15b9-4e49-9dde-f833ffeb111c",
  "messageId": "3fa2cb2a-15b9-4e49-9dde-f833ffeb111c",
  "origin": "NationalLibrary",
  "response": "Please authenticate yourself.",
  "responseChatId": "aef249a4-0eec-49b3-b50f-cce37e9326a3",
  "responseMessageId": "d34ba7f5-150f-4eed-b568-24e62845abb4"
}
```

## API Design

The Mock Bot has a REST API which serves three kinds of clients: end users, DMR-s, CentOps.

### Requests from end user

`POST /chats`

```json
{}
```

`201/Created`

```json
{
  "id": "48307612-d41d-4a34-bc89-ecb5dc992413",
  "messages": [],
  "created": "2022-05-10T04:17:24.975Z"
}
```

---
`GET /chats`

`200/OK`

```json
[
  {
    "id": "48307612-d41d-4a34-bc89-ecb5dc992413",
    "messages": [],
    "created": "2022-05-10T04:17:24.975Z"
  }
]
```

---
`GET /chats/{id}`

`200/OK`

```json
{
  "id": "48307612-d41d-4a34-bc89-ecb5dc992413",
  "messages": [],
  "created": "2022-05-10T04:17:24.975Z"
}
```

---
`POST /chats/{chatId}/messages`

```json
{
  "message": "I would like to renew my id card"
}
```

`201/Created`

```json
{
  "id": "7d887e2f-146f-43fe-a2f0-06efafea6317",
  "message": "I would like to renew my id card",
  "created": "2022-05-10T04:17:25.275Z"
}
```

---

### Requests from DMR

`POST /chats/{chatId}/messages`

```json
{
  "messageIdRef": "7d887e2f-146f-43fe-a2f0-06efafea6317",
  "message": "Please authenticate yourself.",
  "from": "PoliceAndBorderGuard"
}
```

`201/Created`

```json
{
  "id": "c86dd24a-1ab6-458f-9071-02ad4cfc5e37",
  "messageIdRef": "7d887e2f-146f-43fe-a2f0-06efafea6317",
  "message": "Please authenticate yourself.",
  "from": "PoliceAndBorderGuard"
}
```

---

### Requests from CentOps

CentOps notifies Chatbot about changes in the participants of the ecosystem. Initially Chatbot only needs to know about participating DMR-s, but this might
change
once we start encrypting messages between participants (encryption keys might be included in the participant data).

// TODO: just overriding all participants might take quite a bit of bandwidth when there are thousands of them. Discuss alternatives, e.g. CentOps only notifies
that something has changed and chatbot can GET participants?type=DMR&since>=x or just use json merge-patch

`PUT /participants`

```json
[
  {
    "type": "DMR",
    "name": "RIA",
    "status": "Online",
    "url": "https://dmr.ria.ee",
    "created": "2022-05-10T04:17:25.275Z",
    "modified": "2022-05-10T04:17:25.275Z"
  }
]
```

`201/OK`

```
<none>
```

## Hosting

The Mock Bot will be deployed and hosted as a container on a Kubernetes cluster and will follow the foundation design for Kubernetes in terms of
configuration.
