# Wallet API

## Descripción
Wallet API es un servicio para la gestión de billeteras digitales. Permite a los usuarios autenticados realizar operaciones de creación, actualización y eliminación de billeteras, así como transferencias de saldo entre ellas.

---

## Documentación de Endpoints

### **Autenticación**

#### **Registro de usuario**
`POST /api/auth/register`
```json
{
  "username": "usuario123",
  "email": "usuario@example.com",
  "password": "passwordSeguro"
}
```
**Respuesta esperada:**
```json
{
  "message": "Usuario registrado correctamente."
}
```

#### **Inicio de sesión (Login)**
`POST /api/auth/login`
```json
{
  "username": "usuario123",
  "password": "passwordSeguro"
}
```
**Respuesta esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

---

### **Gestión de billeteras**

#### **Crear una billetera** *(Requiere autenticación)*
`POST /api/wallet`
```json
{
  "documentId": "12345678-5",
  "name": "Mi Billetera",
  "balance": 300.00
}
```
**Respuesta esperada:**
```json
{
  "id": 1,
  "documentId": "12345678-5",
  "name": "Mi Billetera",
  "balance": 300.00,
  "createdAt": "2025-03-16T12:00:00Z"
}
```

#### **Obtener todas las billeteras** *(Público)*
`GET /api/wallet`
**Respuesta esperada:**
```json
[
  {
    "id": 1,
    "documentId": "12345678-5",
    "name": "Mi Billetera",
    "balance": 300.00
  }
]
```

#### **Obtener una billetera por ID** *(Público)*
`GET /api/wallet/{id}`
**Respuesta esperada:**
```json
{
  "id": 1,
  "documentId": "12345678-5",
  "name": "Mi Billetera",
  "balance": 300.00
}
```

#### **Actualizar una billetera** *(Requiere autenticación)*
`PUT /api/wallet/{id}`
```json
{
  "name": "Billetera Actualizada"
}
```
**Respuesta esperada:**
```json
{
  "message": "Billetera actualizada correctamente."
}
```

#### **Eliminar una billetera** *(Requiere autenticación)*
`DELETE /api/wallet/{id}`
**Respuesta esperada:**
```json
{
  "message": "Billetera eliminada correctamente."
}
```
#### **Realizar una transferencia** *(Requiere autenticación)*
`POST /api/wallet/transfer`
```json
{
  "sourceWalletId": 1,
  "targetWalletId": 2,
  "amount": 50.00
}
```
**Respuesta esperada:**
```json
{
  "message": "Transferencia realizada con éxito."
}
```

#### **Historial de transacciones** *(Público)*
`GET /api/wallet/{walletId}/transactions`
**Respuesta esperada:**
```json
[
  {
    "id": 1,
    "walletId": 1,
    "amount": -50.00,
    "timestamp": "2025-03-16T12:10:00Z"
  }
]
```
---

## Preguntas clave

### 1⃣ **¿Cómo tu implementación puede ser escalable a miles de transacciones?**
- Se implementó **Entity Framework Core** con **SQL Server**, asegurando optimización en consultas y manejo eficiente de transacciones.
- Uso de **caché en memoria** para evitar accesos innecesarios a la base de datos.
- Se diseñó una **arquitectura modular** y desacoplada para poder escalar horizontalmente.
- **JWT** se usa para autenticación sin estado, lo que permite distribuir la carga en múltiples instancias sin necesidad de sesiones.

### 2⃣ **¿Cómo tu implementación asegura el principio de idempotencia?**
- Se implementan **transacciones ACID** para asegurar que una operación se ejecute solo una vez.
- En operaciones críticas (como transferencias de saldo), se usa un identificador único para evitar la ejecución duplicada.
- Los endpoints de actualización y eliminación requieren validaciones para evitar modificaciones accidentales.

### 3⃣ **¿Cómo protegerías tus servicios para evitar ataques de Denegación de servicios, SQL Injection, CSRF?**
- Uso de **Rate Limiting** para mitigar ataques de Denegación de Servicio (DDoS).
- Sanitización de entradas con **Entity Framework** para prevenir SQL Injection.
- Implementación de **CSRF Tokens** para formularios y restricciones en CORS para prevenir ataques.
- Uso de **CSP (Content Security Policy)** y validaciones en las cabeceras HTTP.

### 4⃣ **¿Cuál sería tu estrategia para migrar un monolito a microservicios?**
- **Identificar dominios y dividir en módulos** con base en responsabilidades.
- **Extraer servicios independientes**, como autenticación y transacciones, en microservicios separados.
- Implementar **eventos asincrónicos** (por ejemplo, con Kafka o RabbitMQ) para sincronizar servicios.
- Utilizar una **API Gateway** para administrar el tráfico y la seguridad.

### 5⃣ **¿Qué alternativas a la solución requerida propondrías para una solución escalable?**
- Uso de **NoSQL (MongoDB o DynamoDB)** si la carga de consultas requiere más flexibilidad.
- Implementación de **gRPC** en lugar de REST para una comunicación más eficiente en alta carga.
- Despliegue en **AWS Lambda o Azure Functions** para escalar dinámicamente según la demanda.
- Uso de **Redis** como caché para optimizar el acceso a datos frecuentes.

---

## **Cómo ejecutar el proyecto**
1⃣ Clonar el repositorio:
```sh
git clone https://github.com/tu-repo.git
cd WalletAPI
```
2⃣ Configurar base de datos en `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=WalletDb;User Id=sa;Password=TuPassword;"
}
```
3⃣ Aplicar migraciones y ejecutar la API:
```sh
dotnet ef database update
dotnet run
```
4⃣ Acceder a Swagger en:
[http://localhost:5215/swagger](http://localhost:5215/swagger)

---

**Desarrollado con .NET 8 y SQL Server**

