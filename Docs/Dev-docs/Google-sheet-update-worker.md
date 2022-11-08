# Google integration

Для визуализации баллов и очередей используется Google Sheets. Выводом результатов в таблицу занимается фоновый процесс, который реагирует на изменение в системе:

- При создании каждого нового сабмишена в процесс генерации добавляется запрос на повторную генерацию очереди по группе, которой принадлежит автор
- При оценивании каждого сабмишена в процесс генерации добавляется запрос на повторную генерацию таблицы с баллами
- При оценивании каждого сабмишена в процесс генерации добавляется запрос на повторную генерацию очереди по группе, т.к. студент уже сдал и его нужно удалить из неё

Фоновый поток имеет задержку в минуту для того, чтобы можно было агрегировать запросы и не спамить большим количеством повторных генераций подряд.

> TODO: написать, что таблицы автоматически создаются и описать что в них должно быть. Можно скриншоты добавить

Примеры получившихся таблиц:

Очередь:

![image](https://user-images.githubusercontent.com/70411602/184488611-f9b7e945-5991-4057-a8b3-f4cc0cf269a2.png)

Баллы:

![image](https://user-images.githubusercontent.com/70411602/184488720-ea5c32a0-4ff6-491d-a2c5-91f74ec61e82.png)

## Details

ITableUpdateQueue, представляемый бэкграунд воркером GoogleTableUpdateWorker содержит в себе методы EnqueueCoursePointsUpdate и EnqueueSubmissionsQueueUpdate, добавляющие курс для обновления его таблицы на листах "Баллы" и "Очередь" соответственно (Создает листы если их не было)
Обновлении таблиц происходит с периодичностью в одну минуту
Также если у курса нет ассоциации с таблицей, создает таблицу в драйве и записывает новую ассоциацию в базу.

Пример использования находится в проекте Playground.Google:

При конфигурации сервисов кроме экстеншиона `.AddGoogleIntegration` необходимо также передать:

- ICultureInfoProvider
- IUserFullNameFormatter
- Базу данных
- Хэндлеры
- Маппинг
- Логгер

И конфигурировать через `options` Google Credentials и DriveId (Id нужной папки).