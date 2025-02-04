# MessageProject – Web-сервис обмена сообщениями

Этот проект представляет собой Web API для обмена сообщениями на **.NET 9** и **PostgreSQL** с WebSocket (SignalR).

- [x] **REST API** для отправки и получения сообщений  
- [x] **WebSocket** для получения сообщений в реальном времени  
- [x] **PostgreSQL** для хранения сообщений  
- [x] **Docker + docker-compose** для быстрого развертывания  
- [x] **Swagger UI** для тестирования API  

## Запуск проекта

### **1. Локально через .NET CLI**

```
dotnet build
dotnet run
```
>Нужно чтобы база данных была уже создана, в данном проекте через запуск dotnet база и таблицы автоматом не создаются

### **2. Запуск через Docker**

```
docker-compose up --build
```

> После запуска API будет доступно по адресу: <http://localhost:5001>

> Swagger доступен по адресу: <http://localhost:5001/swagger/index.html>

## Архитектура проекта

MessageProject/
│── Controllers/         # REST API контроллеры
│   ├── MessageController.cs  # API для работы с сообщениями
│   ├── HomeController.cs     # Главная страница
│
│── DAL/                 # Слой доступа к данным (без ORM)
│   ├── MessageRepository.cs  # Доступ к PostgreSQL
│
│── Hubs/                # WebSocket (SignalR)
│   ├── MessageHub.cs         # Хаб для реального времени
│
│── Models/              # Модели данных
│   ├── Message.cs            # Cтруктура объекта сообщения
│
│── Properties/          # Настройки проекта
│   ├── launchSettings.json   # Настройки запуска (для локального запуска)
│
│── wwwroot/             # Статические файлы
│   ├── js/              # JavaScript-клиенты
│   │   ├── site.js      # WebSocket + API для client1 и client2
│   │   ├── history.js   # API-запрос для client3 (без WebSocket)
│
│── Views/               # Razor-шаблоны (для MVC)
│   ├── Home/Index.cshtml  # Главная страница
│
│── Program.cs           # Точка входа в приложение
│── MessageProject.csproj # Файл конфигурации проекта .NET
│── appsettings.json     # Настройки приложения
│── Dockerfile           # Docker-образ приложения
├── init.sql              # Инициализация БД (автоматическое создание таблицы messages)
│── docker-compose.yml   # Контейнеризация сервиса


## API (REST + WebSocket)

### **1. REST API**

|Метод|Маршрут|Описание|
|:-|:-:|-:|
|POST|/api/messages|Отправить сообщение|
|GET|/api/messages?from=...&to=...|Получить историю сообщений|

Пример отправки сообщения:

```
curl -X POST "http://localhost:5001/api/messages" \
     -H "Content-Type: application/json" \
     -d '{"sequenceNumber":1,"text":"Hello, world!"}'
```

Пример получения истории сообщений за последние 10 минут:

```
curl -X GET "http://localhost:5001/api/messages?from=2025-02-04T17:10:00Z&to=2025-02-04T17:20:00Z"
```

### **2. WebSocket (SignalR)**

|Клиент|Описание|Файл|
|:-|:-:|-:|
|client1.html|Отправка сообщений через API|site.js|
|client2.html|Получение сообщений через WebSocket|site.js|
|client3.html|Запрос истории сообщений через API (без WS)|history.js|

1. WebSocket подключается к /messagehub

2. Сообщения передаются через метод ReceiveMessage

### **3. Маршрутизация (Routing)**

|Маршрут|Описание|Обрабатывается в|
|:-|:-:|-:|
|/	|Главная страница|HomeController.cs|
|/api/messages|	API для сообщений|MessageController.cs|
|/messagehub (WebSocket)|WebSocket-соединение (SignalR)|MessageHub.cs|

### **4. База данных (PostgreSQL)**

В PostgreSQL используется таблица messages

***Структура таблицы:***

```
CREATE TABLE messages (
    id SERIAL PRIMARY KEY,
    sequence_number INT NOT NULL,
    text VARCHAR(128) NOT NULL,
    timestamp TIMESTAMP NOT NULL DEFAULT NOW()
);
```

>Запуск контейнера с PostgreSQL через Docker Compose:

```
docker-compose up --build
```
>Останавливает и удаляет все контейнеры

```
docker-compose down -v
```

>Просмотр всех сообщений в базе данных PostgreSQL через Docker
```
docker exec -it messageproject-db-1 psql -U postgres -d messages -c "SELECT * FROM messages;"

```

>Логирование
```
docker-compose logs -f web
```

