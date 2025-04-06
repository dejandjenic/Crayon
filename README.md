# Crayon

This repository contains solutions for the Technical Exercise and the System Design Exercise.

## Technical Exercise

**Directory structure:**

*   `Crayon` - Contains source code for the API.
*   `Crayons.Tests` - Contains source code for the test project.

### Crayon API

The Crayon API is implemented using the .NET 9 framework as the solution to the technical exercise.

The API uses MariaDB as its primary persistence layer, Redis for caching, and RabbitMQ for message queuing.

#### Dependencies

*   MariaDB
*   Redis
*   RabbitMQ

Configuration for accessing these services can be changed in the `appsettings.json` file within the `Crayon` directory.

The Redis server needs to be configured with the following setting:

```
notify-keyspace-events AKE
```

This is required to receive notifications about keys deleted from the cache. For more details on this caching approach, see this [blog post](https://www.dejandjenic.com/blogs/caching-exploration-in-dotnet.html).

#### Mocking

Communication with the external CCP service is mocked using the [WireMock.Net](https://github.com/WireMock-Net/WireMock.Net) library.

#### Authentication

The system is configured with authentication. Dummy settings are provided in `appsettings.json` for development purposes. For production, these must be replaced with the correct settings for your OAuth server.

Once production OAuth is configured, the `ConfigureJWT` setting in the same file must be set to `false`.

This setting controls whether a dummy JWT token configuration is added to the API (ensuring the API remains protected by a token even in development mode).

Authentication stubbing for development relies on the [TestJWTLibrary](https://www.dejandjenic.com/projects/testjwtlibrary/index.html).

When the API starts with dummy authentication enabled, a default token is generated, printed to the console output, and can be used for subsequent requests.

#### SQL

The SQL schema file (`Schema.sql`) is located in the repository root.
The initial data seeding script (`Seed.sql`) is also located in the repository root.

When running locally for the first time, you need to execute the schema creation script (`Schema.sql`) and, optionally, the data seeding script (`Seed.sql`).

#### Real-time Updates

The API provides real-time updates via a SignalR implementation. It is configured to use a Redis backplane, allowing it to be safely deployed in multi-instance environments (e.g., Kubernetes).

#### Flowcharts

The following flowchart illustrates the process used by these `GET` endpoints:

*   `/inventory`
*   `/accounts`
*   `/accounts/{id}/subscriptions`
*   `/accounts/{id}/subscriptions/{subid}/licences`

This flow is illustrated in the image below:

![Flowchart for common GET endpoints](charts/GetEndpointsFlowChart.png)

**Purchase Order Flowchart**

![Flowchart for Purchase Order endpoint](charts/Purchase.png)

A similar flow applies to other endpoints that modify order data (`cancel`, `change quantity`, and `set expiration`).

### Crayon Tests

XUnit framework is used as the main framework for the test solution.

The main approach for testing was to create functional/integration tests (see [more details](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-9.0)).

This approach doesn't exclude the implementation of unit tests in the future.

#### TestContainers

To create functional tests without relying on local software installations (the dependencies above), the author chose the [TestContainers](https://dotnet.testcontainers.org//) library.

During test setup, Docker containers are created (using the TestContainers library) for the system's dependencies (database, queue, and Redis).

#### Running Tests

Tests can be executed from Visual Studio/Rider or from the command line using:

```
dotnet test
```

Running tests locally assumes that Docker is installed and configured correctly.

If tests are run on a Linux system, you need to ensure that Docker can be run in sudoless mode. If this is not the case, configure this setting by running the command:

```
sudo setfacl -m user:$USER:rw /var/run/docker.sock
```

During the setup phase of testing, one container per dependency is started, and after the tests are finished, those containers are stopped. Test-level isolation is implemented at the database level by having a different database for each test. During the test database setup, the `Schema.sql` and `Seed.sql` files are used to configure both the schema and initial data.

## System design excercise

### Cloud Sales System Design Document

#### 1. Executive Summary

This document details the design for a highly scalable and resilient cloud sales system, enabling Crayon customers globally to efficiently purchase and manage software services from a Cloud Computing Provider (CCP). The system provides both a web portal and a web API for customer access. Leveraging Google Cloud Platform (GCP), Google Kubernetes Engine (GKE) orchestrates a modern microservices architecture, emphasizing scalability, resilience, and maintainability. Security, data consistency, and efficient CCP API integration are core design principles. CCP Webhooks ensure real-time data synchronization. Kubernetes CronJobs handle scheduled tasks. A detailed cost analysis, incorporating network and database bandwidth considerations, supports the architectural choices.

#### 2. Requirements Analysis

The system must meet these functional and non-functional requirements:

*   **Account Management:** Create, read, update, and delete customer accounts and manage their relationship to CCP accounts.
*   **Service Catalog:** Retrieve, store, and provide a regularly updated service catalog from the CCP.
*   **Order Management:** Facilitate service orders, enforcing business rules and order tracking.
*   **Subscription Management:** Enable managing subscriptions (activation, quantity adjustments, cancellation, license extensions).
*   **Invoicing:** Generate accurate invoices for customer purchases, including reseller commissions and supporting different formats.
*   **CCP Integration:** Integrate with the CCP API for account management, order processing, and billing data retrieval. Leverages CCP Webhooks for real-time updates.
*   **Reseller Support:** Accommodate resellers, specifically commission calculations.
*   **Web Portal and API Access:** Provide equivalent functionality via both web portal and API.
*   **Security:** Ensure secure authentication/authorization and data protection.
*   **Scalability:** Support increasing customer and transaction volumes. Projected annual growth of 50% for the next 3 years, ultimately targeting a large, global user base.
*   **High Availability:** Minimize downtime and ensure service resilience. The goal is 99.9% availability.

#### 3. Architectural Overview

The system employs a microservices architecture on Google Kubernetes Engine (GKE).

![text](https://www.plantuml.com/plantuml/png/JP91ZzCm48Nlyoj6ES6XsZi7j2MjgWfMH2qIH-JOGnDjQeF7NgWG_trVx4JTd3BlcvatUSgZ9XDJlavafv0TDuSGBfDmErSD6-L3s_tuzhNZVqp4U7nmmisNO1n_CfFnLcBfc_g2bpL7JMFWWqdonzmA7sVbRSlXynusMTEouF8i1JkJJ6yK9hlbhM1Nz__Eae6wt3-SSrz8aTvkti4hZdrEuhWrTX1-zoJN46-QGfJtPSZEEj5_elf1RGQyIgnmAUX-o_ulxlHcukEkgz_ZVX6juMmszvcxLdjMm9kF28a_yBOxyabmjIQ20Bba0t8Ak5av1KOuL5LW90DublWoMYbDexyGL43a0ZOdnLjhHNNLQr1mqvktl7yHdvG85vN35iDjJWF0Q0rgNqStDHlGERlPgTG0rQic-ZIgFfpaPnGTY9gsxVYCVENQeB6QenNgT-c749xXRU4aaGSn3Z6LFupFPX9T9daRuPV4fXo5nBkrzH7l-4LV0G00)

**Components:**

*   **Google Cloud Load Balancer:** Distributes incoming traffic to GKE nodes.
*   **Istio Ingress Gateway:** Manages traffic routing, security, and observability within the GKE cluster.
*   **GKE Cluster:** Hosts the microservices.
*   **Cloud SQL (PostgreSQL):** Persistent data storage for accounts, orders, subscriptions, etc.
*   **Cloud Pub/Sub:** Asynchronous messaging for inter-service communication and CCP webhook events.
*   **In-Memory Cache (Memorystore):** Improves performance for the Catalog Service.
*   **External IDP (Google Cloud Identity Platform or Auth0):** Provides user authentication.

#### 4. Microservice Breakdown

Microservices are independently deployable and scalable. Each service is responsible for its own data.

##### 4.1 Account Service

*   **Responsibilities:** Manages customer account data, links Crayon and CCP accounts, handles user provisioning (optionally) with the IdP. Subscribes to account changes from the webhook. It *does not* perform authentication or authorization.
*   **API Endpoints:**
    *   `GET /accounts`: List accounts for a customer.
    *   `GET /accounts/{accountId}`: Retrieve account details.
    *   `POST /accounts`: Create a new account.
    *   `PUT /accounts/{accountId}`: Update an account.
*   **Data Model:**
    *   `Account`: `accountId` (UUID), `name` (string), `status` (enum), `contactInformation` (object), `ccpAccountId` (string)
*   **Dependencies:** Cloud SQL (PostgreSQL), External IDP (optional, for provisioning). Cloud Pub/Sub

##### 4.2 Catalog Service

*   **Responsibilities:** Manages the service catalog from CCP, caching for performance. Subscribes to service catalog changes from the webhook.
*   **API Endpoints:**
    *   `GET /services`: Retrieve the service catalog.
    *   `GET /services/{serviceId}`: Retrieve a specific service's details.
*   **Data Model:**
    *   `Service`: `serviceId` (string), `name` (string), `description` (string), `pricing` (object), `features` (array of strings)
*   **Dependencies:** Cloud SQL (PostgreSQL), CCP Integration Service, Memorystore (for caching). Cloud Pub/Sub
*   **Implementation Notes:** Scheduled synchronization runs as a Kubernetes CronJob, updating catalog once per month. Additionally, listens to CCP webhook service catalog events and refresh cache accordingly.

##### 4.3 Order Service

*   **Responsibilities:** Manages order placement and cancellation. Subscribes to order changes from webhook
*   **API Endpoints:**
    *   `POST /orders`: Place a new order.
    *   `GET /orders/{orderId}`: Retrieve order details.
    *   `PUT /orders/{orderId}/cancel`: Cancel an order.
*   **Data Model:**
    *   `Order`: `orderId` (UUID), `accountId` (UUID), `serviceId` (string), `quantity` (integer), `status` (enum), `orderDate` (timestamp)
*   **Dependencies:** Cloud SQL (PostgreSQL), Account Service, Catalog Service, CCP Integration Service. Cloud Pub/Sub
*   **Implementation Notes:** Implements retries with exponential backoff for failed CCP API calls, prioritizes simple over complex Saga pattern. The reconciliation is handled by dead letter queue.

##### 4.4 Subscription Service (or License Service)

*   **Responsibilities:** Manages subscriptions (licenses), including quantity adjustments, cancellations, and license extensions. Listens for subscription changes from the webhook.
*   **API Endpoints:**
    *   `GET /subscriptions`: Retrieve a list of subscriptions for an account.
    *   `GET /subscriptions/{subscriptionId}`: Retrieve a specific subscription's details.
    *   `PUT /subscriptions/{subscriptionId}/quantity`: Update subscription quantity.
    *   `PUT /subscriptions/{subscriptionId}/cancel`: Cancel subscription.
    *   `PUT /subscriptions/{subscriptionId}/extend`: Extend license.
*   **Data Model:**
    *   `Subscription`: `subscriptionId` (UUID), `accountId` (UUID), `serviceId` (string), `quantity` (integer), `status` (enum), `startDate` (timestamp), `endDate` (timestamp)
*   **Dependencies:** Cloud SQL (PostgreSQL), Account Service, Order Service, CCP Integration Service. Cloud Pub/Sub

##### 4.5 Invoice Service

*   **Responsibilities:** Generates invoices by retrieving billing data from the CCP API, manages invoice payments, and stores generated invoice documents in GCS.
*   **API Endpoints:**
    *   `GET /invoices`: Retrieve invoices for a customer.
    *   `GET /invoices/{invoiceId}`: Retrieve a specific invoice.
    *   `POST /invoices/generate`: Manually trigger invoice generation.
*   **Data Model:**
    *   `Invoice`: `invoiceId` (UUID), `accountId` (UUID), `invoiceDate` (timestamp), `charges` (decimal), `discounts` (decimal), `totalAmount` (decimal), `status` (enum), `paymentInformation` (object)
*   **Dependencies:** Cloud SQL (PostgreSQL), CCP Integration Service, Google Cloud Storage.

##### 4.6 CCP Integration Service

*   **Responsibilities:** Abstraction and translation layer for the CCP API. Handles authentication, error handling and maps from CCP API specific objects to generic internal ones. Listens and handles events from CCP Webhooks and Publishes events to others.
*   **Methods:**
    *   `getServiceCatalog()`: Retrieves the service catalog.
    *   `createAccount(accountInfo)`: Creates a CCP account.
    *   `placeOrder(orderInfo)`: Places a service order.
    *   `manageSubscription(subscriptionId, action, parameters)`: Manages subscriptions (activate, cancel, update).
    *   `getBillingData(accountID, period)`: Retrieves billing information for invoicing.
*   **Data Model:** Translates between CCP API and internal formats.
*   **Dependencies:** CCP API client library, external configuration. Implements circuit breaker and retry mechanisms. Cloud Pub/Sub

##### 4.7 Notification Service

*   **Responsibilities:** Sends notifications (email, SMS) about key events (order confirmation, invoice generation, etc.).
*   **API Endpoints:** (Internal)
    *   `POST /notifications`: Send a notification.
*   **Data Model:** Manages preferences.
*   **Dependencies:** External notification providers (SendGrid, Twilio, etc.).

##### 4.8 Support Case Service

*   **Responsibilities:** Manages the support cases.
*   **API Endpoints:**
    *   `GET /supportcases`: Retrieve a list of support cases.
    *   `POST /supportcases`: Create a support case.
*   **Data Model:** Includes support case info and history.
*   **Dependencies:** Cloud SQL (PostgreSQL), Account Service.

#### 5. GKE Deployment Strategy

*   **Containerization:** Each microservice is packaged as a Docker container using Dockerfiles.
*   **Deployment:** Kubernetes Deployments manage desired state and rolling updates.
*   **Services:** Kubernetes Services expose microservices for internal communication.
*   **Ingress:** Istio Ingress Gateway exposes selected microservices to the outside world.
*   **Resource Management:** CPU and memory limits and requests are configured for each deployment. Resource utilization must be measured and configured per microservice.
*   **Health Checks:** Liveness and readiness probes ensure pod health.
*   **Kubernetes CronJob:** A Kubernetes CronJob is used to schedule catalogue sync.

#### 6. Networking and Security

*   **Google Cloud Load Balancer:** External load balancer distributes traffic across GKE nodes.
*   **Istio:** Provides service mesh capabilities: mTLS, traffic management, authorization.
*   **Authorization:** JWT-based authorization handled by Istio, using claims from the external IdP. No code changes are necessary in internal microservices.
*   **Network Policies:** Kubernetes Network Policies restrict inter-service traffic.
*   **Secrets Management:** Kubernetes Secrets store credentials; consider more secure solutions like HashiCorp Vault.
*   **Virtual Private Cloud (VPC):** All resources must be deployed to VPC.

#### 7. Data Management

*   **Database:** Cloud SQL (PostgreSQL) stores persistent data. PostgreSQL chosen because it is cloud-native and cost-effective solution.
*   **Data Model:** Defined in each microservice.
*   **Data Consistency:** Prioritizes eventual consistency; uses asynchronous communication (Pub/Sub) for propagation. Uses of CCP Webhooks allows us to keep consistency and make services asynchronous.
*   **Backups:** Cloud SQL manages backups for all data.

#### 8. Monitoring and Logging

*   **Cloud Monitoring:** Collects metrics (CPU, memory, API response times, error rates).
*   **Cloud Logging:** Aggregates microservice logs, structured for easy searching.
*   **Alerting:** Triggers alerts on critical issues.
*   **Tracing:** Cloud Trace tracks requests to identify performance bottlenecks and improve service meshing.

#### 9. Scalability and High Availability

*   **Horizontal Scaling:** Microservices scaled horizontally via GKE.
*   **Autoscaling:** Horizontal Pod Autoscaler (HPA) adjusts replicas based on utilization.
*   **Load Balancing:** Google Cloud Load Balancer and Istio share load.
*   **Multi-Zone:** GKE is deployed across multiple zones.
*   **Database Replication:** Read replicas increase read speed, provide backup for data consistency.

#### 10. Cost Analysis (Detailed)

This section provides a preliminary cost estimate based on the assumption of a heavily used global service. Actual costs may vary depending on the actual usage patterns and GCP pricing.

**10.1 Usage Assumptions:**

*   **Customers:** 1 million customers, growing at 50% annually.
*   **Accounts:** 5 accounts per customer.
*   **Orders:** 10 orders per account per month.
*   **Subscriptions:** 3 subscriptions per account.
*   **API Requests:** 10,000 RPS during peak hours.
*   **Data Transfer (Outbound):** 10 TB per month.
*   **Data Transfer (Inbound)** 1 TB per month.
*   **Invoice Storage:** 5 TB with an estimate invoice size of 500KB.

**10.2 Service Costs:**

| Service                      | Instance/Tier | Description                                                           | Monthly Cost (Estimate) |
| ---------------------------- | ------------- | --------------------------------------------------------------------- | ----------------------- |
| GKE Cluster                  | Standard        | 15 nodes, n1-standard-4 (4 vCPUs, 15 GB memory)                     | $3,000                 |
| Cloud SQL (PostgreSQL)       | db-n1-standard-2 | 2 vCPUs, 7.5 GB memory, 1 TB storage, with 1 read replica            | $500                    |
| Cloud Pub/Sub                | -              | Based on 1 Billion Messages, will scale as well per the rate | $200             |
| Google Cloud Storage (GCS)    | Standard        | 5 TB Storage, based on reads  | $150                    |
| Google Cloud Interconnect        | Tier 1 Dedicated Connection      | Data is being passed in and out of the system and will scale          | $10,000                  |
| External IDP (Auth0)          | -              | Auth0 has multiple service plans based on usage and number of active users             | Contact Vendor                  |
| Network (Outbound Traffic) |                  | Total Outbound Traffic is about 10TB, it will increase the costs             | 800    |
| Logging and Monitoring                       |   Cloud Logging, trace and related services           | Assuming that 1TB of logs are being produced                  | 1000                  |

**Total Estimated Monthly Infrastructure Cost: ~\$15,650**

**Notes:**

* All prices are estimates based on GCP's pricing calculator.
* This analysis can be greatly optimised based on instance reservations, but is not considered here to focus on general understanding.
* Does not include development, security and operational personnel costs.
* All estimates are based in United States with prices that apply to zones in US. Prices for different places can vary.
* Traffic estimates are for worst case for given month.
* Network calculation is based only on traffic volume, not including number requests. In high-usage systems it is better to also calculate number of transactions.

#### 11. Sequence Diagram (Create New Order)

Illustrates flow with External IDP JWT validation, and removes authentication responsibility from the Account Service.

![text](https://www.plantuml.com/plantuml/png/VPB1ZXen38RlUGgBysvxvr0jcbsXgIhDg5Ervp0n43MJJ9q4ojlNiQQpXCDU2DYV_V-sUOb9KygJKpE_ThIpzWcQ75EOa417QAhCfet91cZzYJ16sEk4LttR5B3Tr-JRtuJajOFsjHU0tvhOZMF8_6L0kjWH1TeDDTEHGQg8xe5ecfxz93oHPck-OZdNFj0kP0DzFdmUya6OVb2gWUSlFC0MlkEVZ3512Z0IweIWmUCLGh6Xshrmhtq1ZIKSXMnD3yT0e7CwesSjCQAOAtYpXLsLAEoachKL6vbiAdgcGDh5kz9LvshJ6Rx--l7ezIuZzYJr4yaURoK8qPwyJfdm2MoC6Uc9VHgBVaIbNHBcefqr2a0Qy0csy2s4tta6lLpeS8CSaTWl72bCeWOCmlCYk5j0wyjmCkRQhnlM6MMDG2Z65N2A0RdI5XhPT_cnR1jOFjdB4cmLo5CAUkuuvuEpyRoq7CCqEvJjfW2maUYd9OeROC6C2hZoloCV4g-eBiTLGo3OlhjP_Ww62OEFlhhtHNJtCFVrHrlleypx8OOkebp5-lekNFP650WcZ56VKA4tweK_ykJ-0G00)

#### 12. PlantUML Component Diagram

![text](https://www.plantuml.com/plantuml/png/VPFFQlCm4CJlUeh5fto7yBr2WWibq3ykFlJGzY1B6rTWQusq2eJITwzYDnXtJMxHp2-BHhFUHTQ1KzyfJnQXA3jA5chTOOJw71dx0huKm5kP8bEF0LwnWOe2w-vZeU-hmoZiZQ7a6MeC9sTmWdJ6gLrepy5YM2XrQg89Rc17Vc4S_8dox-N8ieA3Pso35lmJiJiw8uqw3GC5Xb97d8rlfQnctUWuFy3BmxlVL1Iv3PWF_mhG4MoZp6WNzqHjX_0BCmNTueYq9bwPoQnIypy76yqX1qppZ4i2YvuTdw7gD1yfz2Fdx725xbpau9h4QA7KvXDXyuWzXLmD1PokPIwwKgiLmNRxjwFbCGEoCyd83YKZEfN4UiSI59slCkjrAX4dmvVkH8HR-lYeeAP8uXyoncoAgqywh1kjYQ1MaipprCgkpy2qKt0J6XTABJyBEKFjqDlKTpy0)
