# Stop and remove existing api container
docker stop funbooksandvideos-api
docker rm funbooksandvideos-api

# Build new image
docker build -f FunBooksAndVideos.API/Dockerfile -t funbooksandvideos-api .

# Run container
docker run -d -p 5000:5000 --name funbooksandvideos-api funbooksandvideos-api

# Check logs of specific container
docker logs funbooksandvideos-api

# Health check for the entire flow
curl http://localhost:5000/health

# Delete all containers and volumes 
docker compose down -v

# Build all images and run containers
docker compose up --build -d