# YostinMotors

API REST desarrollada con .NET 10 y PostgreSQL, ejecutada en contenedores Docker.
Proyecto de venta de motocicletas para la materia Tecnología Web 1.

## Autor de este repositorio
- **Integrante:** Yeltsin Pool Sardina Bautista
- **Tabla trabajada:** Cliente
- **Controlador:** `ClientesController`

## Equipo y tablas asignadas

| Integrante | Tabla | Repositorio |
|---|---|---|
| Yeltsin Sardina | Cliente | https://github.com/Wakas4/Tarea4-TecnologiaWeb |


## Tecnologías
- .NET 10 (Web API) con Entity Framework Core
- PostgreSQL
- Docker y Docker Compose
- Scalar (documentación de la API)

## Estructura del proyecto
```
YostinMotors/
├── contructor.yml
├── despliegue.yml
├── .env.example
├── .gitignore
├── README.md
├── script.sql
└── YostinMotors/
    ├── YostinMotors.csproj
    ├── Program.cs
    ├── Dockerfile
    ├── .dockerignore
    ├── Controllers/
    ├── Models/
    ├── Data/
    └── Migrations/
```

## Requisitos
- Docker y Docker Compose
- Git

## Ejecución desde el código fuente
1. Clonar el repositorio:
   ```bash
   git clone git@github.com:Wakas4/Tarea4-TecnologiaWeb.git
   cd Tarea4-TecnologiaWeb
   ```
2. Crear el archivo `.env` a partir del ejemplo y editar los valores:
   ```bash
   cp .env.example .env
   ```
3. Construir y levantar los contenedores:
   ```bash
   docker compose --env-file .env -f contructor.yml up --build
   ```
4. Abrir la documentación de la API en http://localhost:8080/scalar/v1

Para detener los contenedores:
```bash
docker compose --env-file .env -f contructor.yml down
```
No usar `-v`, porque borra el volumen `pg_data` y se pierden los datos.

## Configuración
La API lee estos valores desde variables de entorno (definidas en `.env`):

| Variable | Descripción |
|---|---|
| `POSTGRES_DB` | Nombre de la base de datos |
| `POSTGRES_USER` | Usuario de PostgreSQL |
| `POSTGRES_PASSWORD` | Contraseña de PostgreSQL |
| `API_PORT` | Puerto publicado en el equipo |

La cadena de conexión se pasa al contenedor con `ConnectionStrings__Connection`
y usa `postgres` (el nombre del servicio) como host. El puerto de escucha se
define con `ASPNETCORE_URLS`. Las credenciales no quedan dentro de la imagen.

## Endpoints de Cliente

| Método | Ruta | Descripción |
|---|---|---|
| POST | /api/Clientes | Crear cliente |
| GET | /api/Clientes | Listar clientes |
| GET | /api/Clientes/{id} | Obtener cliente por ID |
| PUT | /api/Clientes/{id} | Actualizar cliente |
| DELETE | /api/Clientes/{id} | Eliminar cliente |

Ejemplo de cuerpo para POST:
```json
{
  "nombre": "Juan",
  "apellido": "Perez",
  "ci": "1234567",
  "correo": "juan@correo.com",
  "telefono": "70000000"
}
```

## Base de datos
Las tablas se crean con las migraciones de Entity Framework, que se aplican
automáticamente al iniciar la API. El SQL equivalente está en `script.sql`.
Los datos se guardan en el volumen nombrado `pg_data`, por lo que se conservan
al detener y volver a levantar los contenedores.

## Docker Hub
- Repositorio: https://hub.docker.com/r/yeltsinsardina/tarea4
- Etiqueta: `yeltsinsardina/tarea4:v1.0`
- Descarga:
  ```bash
  docker pull yeltsinsardina/tarea4:v1.0
  ```

## Despliegue sin compilar
`despliegue.yml` usa la imagen publicada en Docker Hub, sin necesidad del
código fuente. Solo requiere ese archivo y un `.env`:
```bash
cp .env.example .env
# editar .env con valores reales (por ejemplo POSTGRES_USER=admin)
docker compose --env-file .env -f despliegue.yml up -d
```
Probar: http://localhost:8080/api/Clientes

## Uso de IA (paso 6)

### Qué consulté
Usé Gemini para preparar `despliegue.yml`. El prompt fue:

> Tengo una imagen publicada en Docker Hub: yeltsinsardina/tarea4:v1.0, que es una
> API .NET 10 que se conecta a PostgreSQL. Necesito un archivo despliegue.yml de
> Docker Compose que levante la API y PostgreSQL SIN compilar desde código fuente.
> Requisitos: el backend usa image (no build), una red compartida, un volumen
> nombrado para PostgreSQL, variables tomadas de un archivo .env (POSTGRES_DB,
> POSTGRES_USER, POSTGRES_PASSWORD, API_PORT), y la cadena de conexión
> ConnectionStrings__Connection usando el nombre del servicio de PostgreSQL como host.

### Qué propuso
Un compose con dos servicios: `postgres:16-alpine` con healthcheck, y la API con
`image: yeltsinsardina/tarea4:v1.0` y `depends_on` con condición `service_healthy`.
Incluía la red `app-network`, el volumen `pgdata` y un `.env` de ejemplo con las
cuatro variables.

### Cómo verifiqué la propuesta
- Confirmé que el backend usa `image:` y no `build:`.
- Confirmé que el host de la conexión es `postgres` y que usuario, contraseña y
  base salen del `.env`, sin valores escritos en el archivo.
- Confirmé que el volumen `/var/lib/postgresql/data` es la ruta correcta para
  `postgres:16`.
- Quité `version: '3.8'`, que está obsoleto en Docker Compose v2.
- Cambié `ASPNETCORE_ENVIRONMENT` de Production a Development, porque en
  Production no se habilita Scalar.
- Agregué `ASPNETCORE_URLS` para configurar el puerto de escucha por variable de
  entorno, como pide la consigna.
- Cambié los nombres de contenedor para evitar conflicto con un contenedor de
  prueba que ya existía.
- Probé en una carpeta limpia con solo `despliegue.yml` y `.env`: Compose descargó
  la imagen desde Docker Hub y `GET /api/Clientes` respondió correctamente.

## Flujo de trabajo en Git
Desarrollé todo en la rama Main ya que todo el trabajo se subio en una sola pieza.

## Problemas encontrados y soluciones
- **Conflictos de versiones de paquetes (NU1202 / NU1605):** la última versión de
  EF Core es para net11.0. Se fijaron EF Core, EF Core Design y `dotnet-ef` en
  10.0.9 para que fueran compatibles con net10.0 y con Npgsql.
- **Generador de controladores:** `--api` no existía, se usó `-api` y se indicó el
  proveedor con `-dbProvider postgres`.
- **Puerto 5432 ocupado:** el PostgreSQL local usaba el puerto, se mapeó `5433:5432`
  en el contenedor de prueba.
- **Dockerfile:** `dotnet publish --no-build` fallaba con `-o /app/build`; se quitó
  `--no-build`.
- **`.env` subido por error a GitHub:** se quitó con `git rm --cached`, se reescribió
  el commit y se cambió la contraseña.