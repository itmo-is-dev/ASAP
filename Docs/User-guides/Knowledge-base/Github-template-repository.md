# Шаблонный репозиторий

Для большинства дисциплин, которые предполагают написание кода, можно и имеет смысл заранее создавать шаблон проекта. Такой шаблон будет предоставляться студентам, чтобы они его дописывали в рамках выполнения работ. GitHub поддерживает возможность создания шаблонного репозитория. Для этого достаточно создать обычный git-репозиторий, загрузить его на GitHub и предоставить ссылку администратору системы. Администратор загрузит этот шаблон в организацию, и студенты будут создавать свои репозитории из этого шаблона. Шаблон позволяет:

- Определить инструменты. Например, созданный проект будет содержать код с использованием .NET 6, а значит студенты не смогут случайно использовать неправильную версию;
- Определить структуру решения. Если в шаблонном репозитории уже созданы директории под каждую работу, то уменьшается вероятность, что они задумаются о создании отдельных репозиториев на каждую работу;
- Добавление проверок и тестов. GitHub позволяет задать для репозитория поведение при создании Pull request. Таким поведением может являться проверка кода встроенным в язык/систему сборки анализатором. Например, компилятор .NET'а позволяет не только проверить, что проект не содержит ошибок компиляции, но и сразу получить предупреждения о проблемах в коде - нарушения код стайла, потенциальные null reference exception и другое. Также используя CI можно настроить автоматический запуск тестов при создании Pull request. Это даст гарантию того, что студент загружает правильный код.