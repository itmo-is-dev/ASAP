using CommandLine;
using ITMO.Dev.ASAP.Commands.CommandVisitors;

namespace ITMO.Dev.ASAP.Commands.SubmissionCommands;

[Verb("/help")]
public class HelpCommand : ISubmissionCommand
{
    public const string HelpString = @"
Команды:

- `/help`
- `/deactivate` - перевод сабмишена в неактивное состояние, чтобы обозначить, что он не готов к сдаче
- `/activate` - перевод сабмишена обратно в активное состояние
- `/create-submission` - команда на случай, если был создан PR, но сабмишен не был автоматически создан
- `/delete` - удаление сабмишена. Стоит использовать только в ситуациях, когда сабмишен был создан случайно.

Команды только для преподавателя:

- `/rate [RatingPercent] {ExtraPoints}`. Например, `/rate 90 2` - это 90% баллов за работу +2 дополнительных баллов;
- `/update [Submission number] {-r RatingPercent} {-e ExtraPoints} {-d Date}`. Например:
  - `/update 1 -d 27.09.2022` обновит сабмишен с номером 1 и поставит ему дату 27.09
  - `/update 2 -r 100 -e 5` обновит сабмишен с номером 2 и поставит ему 100% рейта и 5 дополнительных баллов
- `/ban {Submission number}` - забанить сабмишен. Сабмишен будет помечен как забаненный, но не удален из базы данных. 
Без указания номера сабмишена, будет забанен последний сабмишен. 

Обрабатываемые ивенты:

- Создание или переоткрытие PR - создаёт сабмишен
- Добавление нового комита в PR - обновляет дату сабмишена на текущую";

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }
}