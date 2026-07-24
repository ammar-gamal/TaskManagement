restore:
	dotnet restore

build: restore
	dotnet build --no-restore

run:
	dotnet run --project src/TaskManagement/TaskManagement.csproj

test: test-unit test-integration

test-unit:
	dotnet test tests/TaskManagement.UnitTests/TaskManagement.UnitTests.csproj

test-integration:
	dotnet test tests/TaskManagement.IntegrationTests/TaskManagement.IntegrationTests.csproj

migrate:
	dotnet ef database update --project src/TaskManagement/TaskManagement.csproj

docker-up:
	docker-compose up --build

docker-down:
	docker-compose down -v

clean:
	dotnet clean