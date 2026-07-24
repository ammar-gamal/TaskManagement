# Error Responses

All errors returned by all APIs follow a consistent structure to make error handling easier for clients.

All APIs return errors as **Problem Details** objects, following the [RFC 9457](https://www.rfc-editor.org/rfc/rfc9457) format.

---

# Response Format

```json
{
  "type": "about:blank",
  "title": "Error Title",
  "status": 400,
  "detail": "A detailed description of the error.",
  "instance": "/api/projects",
  "requestId": "0HNF2M4JQ9B8A:00000001",
  "traceId": "00-..."
}
```

## Response Properties

| Property | Type | Description |
|----------|------|-------------|
| `type` | string | URI identifying the error type. |
| `title` | string | Short description of the error. |
| `status` | integer | HTTP status code. |
| `detail` | string | Detailed explanation of the error. |
| `instance` | string | The request path where the error occurred. |
| `requestId` | string | Unique identifier for the HTTP request, useful for correlating application logs. |
| `traceId` | string | Unique identifier used for troubleshooting. |

---

# Validation Error Response

When request validation fails, the response includes an additional `errors` property containing validation messages.

Example:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "See the errors property for additional details.",
  "instance": "/api/auth/register",
  "requestId": "0HNF2M4JQ9B8A:00000002",
  "traceId": "00-...",
  "errors": {
    "username": [
      "The Username field is required."
    ],
    "password": [
      "The Password field is required."
    ]
  }
}
```

---

# HTTP Status Codes

All the APIs use the following HTTP status codes:

| Status Code | Description |
|-------------|-------------|
| **400 Bad Request** | The request is invalid or contains invalid data. |
| **401 Unauthorized** | Authentication is required or authentication failed. |
| **404 Not Found** | The requested resource does not exist. |
| **409 Conflict** | The request conflicts with the current state of the resource. |
| **500 Internal Server Error** | An unexpected server error occurred. |

---

# Content Type

Error responses are returned using:

```http
Content-Type: application/json
```

---

# Example

Request:

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "ammar",
  "password": "12345"
}
```

Response:

```http
HTTP/1.1 401 Unauthorized
Content-Type: application/json
```

```json
{
  "type": "about:blank",
  "title": "Invalid Credentials",
  "status": 401,
  "detail": "Username or password is incorrect.",
  "instance": "/api/auth/login",
  "requestId": "0HNF2M4JQ9B8A:00000003",
  "traceId": "00-abc123"
}
```
