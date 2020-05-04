# Receiving Web Hook Calls from Mixer

## General Approach

See [Mixer Web Hooks](https://dev.mixer.com/reference/webhooks)

- Register an Event
  - POST https://mixer.com/api/v1/hooks
  - Secured by OAuth Secret
  - Store the response
- Create a function to handle the hook
  - Validate the request signature
  - Must handle duplicate events
- Renew webhook every 90 days
  - Timer based function
  - Use renewalUrl provided in registration response
