# checkout

Repository contains inital phase of payment getway and simple acquiring bank simulator.

Payment getway details:
 - REST API
 - built using .NET 5
 - swagger (swashbuckle) as API documentation
 - Onion architecture (a
 - DDD
 - db is mocked
 - duplicates checker is mocked
 - acquiring bank(s) configuration is mocked
 - simple circuit breaker and retries
 - simple resilient http client for macking calls to an acquiring bank is added
 - merchant_id is added as a header parameter for simplicity. When there is an authentication it's better to get it from jwt or other token types
 - simple card and currency validations where added. There are comments left in the code where it should be checked whenther currency supported/exist and card validation/fraud checking
 - tests weren't addded due to time reason

Acquirung bank details:
  - REST API
  - built using .NET 5
  - mocked endpoint for processing payment requests with simple responses

To test a solution should be run using either docker-compose or visual studio/rider.

Once application is up and running use `https://localhost:5000` to test it.

Assumptions:
- there is no info about rps, latency, read/write ratio and etc. so it's built on assuption that a load isn't high (not millions/billions of requests). Based on the load design/architecture could be completely different (e.g. separate read/write api services, messaging, caching, sharding, replication and etc.)
- no 3d secure or whatever that need additional user input. just direct call to acquiring bank. API for it would have different workflow and isn't implemented here
- we should provide merchant id in any form to associate a payment with it. In this example it is provided via http header as a guid for simplicity. It's better to figure it out from auth token (jwt, or whatever)

Improvements (time is needed for them):
- adding healthchecks endpoint(s)
- areas in the code with left comments (some logic should be improved, implemented)
- cover logic with unit and integration tests
- add all kind of validation that is mocked, or where comments are left
- better circuit breaker (per endpoint)
- unified error responses
- implement left extra mile points
