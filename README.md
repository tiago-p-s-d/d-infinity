# D-Infinity - Backend Setup

This project is built with .NET 9/10 and Entity Framework Core, running entirely within Docker. We use a .env file to manage sensitive credentials (JWT keys and SMTP settings) securely.

## Environment Configuration

To ensure security, sensitive information is stored in a local .env file which is ignored by Git. Follow these steps to set up your environment:
### 1. Create the Environment File

In the root directory of the project (where docker-compose.yml is located), create a file named .env

### 2. Configure Credentials

Open the .env file and add the following variables. Replace the placeholders with your actual data:
```bash
# --- Database Configuration ---
DB_ROOT_PASSWORD=db_infinity
DB_NAME=d_infinity_db

# --- JWT Authentication ---
JWT_KEY=your-secure-32-character-key
JWT_ISSUER=d-infinity-api
JWT_AUDIENCE=d-infinity-angular

# --- Email Service (Gmail SMTP) ---
# Use a Google App Password (16 digits), NOT your regular password.
EMAIL_USER=your-email@gmail.com
EMAIL_PASS=your-16-digit-app-password
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
```


### 3. Build and Run with Docker

Once the .env file is ready, you don't need to install .NET or MySQL locally. Simply run:

```bash
# Build and start all services
docker-compose up --build
