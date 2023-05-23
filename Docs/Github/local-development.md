# Local development

Для локальной разработки рекомендуется использовать свою тестовую организацию.
Данный документ описывает шаги, необходимые для этого.

## Создание организации

Создайте тестовую организацию, к которой будет привязан SubjectCourse

## Создание приложеня (бота)

Перейдите в настройки организации

![](local-development/create-bot-goto-settings.png)

Перейдите в меню `Developer settings GitHub Apps`

![](local-development/create-bot-goto-github-apps-settings.png)

Создайте новое приложение

![](local-development/create-bot-click-new-app.png)

В качестве ссылки укажите ссылку на свою тестовую организацию

![](local-development/create-bot-enter-bot-info.png)

Отключите Webhook'и, их можно будет включить дальше, используя [smee](https://smee.io) для того чтобы получать 
вебхуки на локальную машину

<div align="center">
  <div style="display: flex;flex-direction:row">
    <img src="local-development/create-bot-webhook-enabled.png" style="width:50%"/>
    <img src="local-development/create-bot-webhook-disabled.png" style="width:50%">
  </div>  
</div>

Добавьте `Read and write` permission'ы для 

### Repository permissions
- Administration
- Commit statuses 
- Contents
- Discussions
- Issues
- Packages
- Pull requests

### Organization permissions
- Members
- Team discussions 

Выберите что, приложение может быть установлено только в данной организации

![](local-development/create-bot-allow-only-this-account.png)

Создайте приложение

Добавьте и скачайте приватный ключ

![](local-development/create-bot-generate-private-key.png)

## Заполнение кофигурации

Откройте `.NET User Secrets` проекта `ITMO.Dev.ASAP`

![](local-development/appsettings-open-user-secrets.png)

Скопируйте id вашего приложения 

![](local-development/appsettings-app-id.png)

Добавьте свою организацию как service installation

```json
{
  "Github": {
    "Octokit": {
      "AppId": {APP_ID},
      "PrivateKey": "{YOUR_PRIVATE_KEY}",
      "Service": {
        "Organization" : {
          "Enabled" : true,
          "Name": "{YOUR_ORGANIZATION_NAME}"
        }
      }
    }
  }
}
```

## Настройка smee.io для локального получения веб-хуков

smee.io – reverse proxy сервис для обработки вебхуков. Используется для того, чтобы GitHub отправлял 
вебхуки вам на локальную машину, без надобности подключения статического IP адреса, сертификатов.   

Переходим на сайт smee.io

Ставим клиент smee `npm install --global smee-client` (нужна установленная node)

В качестве url можно использовать `https://smee-io/{SMEE_CHANNEL_ID}`, где {SMEE_CHANNEL_ID} любая строка-идентификатор, 
поэтому рекомендуется запомнить используемую ссылку, чтобы не менять её в настройках приложения GitHub

> Обратите внимания на то, что поток даных, приходящих к сервис **не приватный**, любой пользователь, знающий ссылку 
> может подключиться и просматривать данные веб-хуков идущих от вашей организации

Далее нужно забиндить поток данных, получаемых smee на локальный url приложения ASAP, обрабатывающий веб-хуки GitHub

```smee -u https://smee.io/{SMEE_CHANNEL_ID} -t http://{LOCAL_URL+PORT}/api/github/webhooks```

> Значение {LOCAL_URL+PORT} можно получить при запуске приложения из консоли или из launchSettings.json

> Не забывайте заново биндить smee после перезапуска локальной машины