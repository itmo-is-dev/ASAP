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

Отключите Webhook'и, их можно будет включить дальше, используя [ngrok](https://ngrok.com) для того чтобы получать 
вебхуки на локальную машину

<div style="display: flex;">
<img src="local-development/create-bot-webhook-enabled.png"/>
<img src="local-development/create-bot-webhook-disabled.png">
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
      "PrivateKey": "{YOUR_PRIVATE_KEY",
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