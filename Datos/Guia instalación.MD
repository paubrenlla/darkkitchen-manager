# DarkKitchen — Deploy

## Requisitos
- Docker Desktop instalado

## Pasos

1. Abrir una terminal en esta carpeta (Aplicacion/)

2. Cargar las imágenes:
   
   docker load < api.tar
   
   docker load < frontend.tar

3. Levantar todo:
   docker-compose up

4. Esperar hasta ver "Now listening on: http://[::]:8080" en los logs

5. Abrir el browser en:
   - Frontend: http://localhost:8080
   - API:      http://localhost:5254

## Para bajar todo
   docker-compose down