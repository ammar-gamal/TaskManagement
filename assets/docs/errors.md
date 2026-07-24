# Error Responses

This API uses the **Problem Details for HTTP APIs** format (`application/problem+json`) for all error responses.

All errors returned by the API follow a consistent structure to make error handling easier for clients.

---

# Response Format

```json
{
  "type": "about:blank",
  "title": "Error Title",
  "status": 400,
  "detail": "A detailed description of the error.",
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
| `traceId` | string | Unique request identifier used for troubleshooting. |

---

# Validation Error Response

When request validation fails, the response includes an additional `errors` property containing validation messages.

Example:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "username": [
      "The Username field is required."
    ],
    "password": [
      "The Password field is required."
    ]
  },
  "traceId": "00-..."
}
```

---

# HTTP Status Codes

The API uses the following status codes:

| Status Code | Description |
|-------------|-------------|
| **400 Bad Request** | The request is invalid or contains invalid data. |
| **401 Unauthorized** | Authentication is required or authentication failed. |
| **403 Forbidden** | The authenticated user is not allowed to perform this operation. |
| **404 Not Found** | The requested resource does not exist. |
| **409 Conflict** | The request conflicts with the current state of the resource. |
| **429 Too Many Requests** | The client has sent too many requests in a given time period. |
| **500 Internal Server Error** | An unexpected server error occurred. |

---

# Content Type

Error responses are returned using:

```http
Content-Type: application/problem+json
```

---

# Example

Request:

```http
POST /api/auth/login
Content-Type: application/json
```

Response:

```http
HTTP/1.1 401 Unauthorized
Content-Type: application/problem+json
```

```json
{
  "type": "about:blank",
  "title": "Invalid Credentials",
  "status": 401,
  "detail": "Username or password is incorrect.",
  "traceId": "00-abc123"
}
```
