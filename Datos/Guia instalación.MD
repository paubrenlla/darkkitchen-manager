# Guía de Instalación — DarkKitchen

## Requisitos

- Docker Desktop instalado y corriendo
- El ejecutable de la aplicación (carpeta `Aplicación`)

---

## Paso 1 — Levantar la base de datos

levantar el docker en la carpeta Datos:

```bash
docker-compose up -d
```

Esto levanta un contenedor con la base de datos

---

## Paso 2 — Ejecutar la aplicación

Entrá a la carpeta `Aplicación` y ejecutá el ejecutable


---

## Paso 3 — Verificar que funciona

Abrí el navegador y navegá a:
http://localhost:5000/swagger