from abc import ABC, abstractmethod


class Workable(ABC):
    @abstractmethod
    def work(self):
        pass


class Eatable(ABC):
    @abstractmethod
    def eat(self):
        pass


class Sleepable(ABC):
    @abstractmethod
    def sleep(self):
        pass


class HumanWorker(Workable, Eatable, Sleepable):
    def __init__(self, hours):
        self.worker_type = "human"
        self.hours = hours

    def work(self):
        print(f"{self.worker_type} working for {self.hours} hours")

    def eat(self):
        print("human eating")

    def sleep(self):
        print("human sleeping")


class RobotWorker(Workable):
    def __init__(self, hours):
        self.worker_type = "robot"
        self.hours = hours

    def work(self):
        print(f"{self.worker_type} working for {self.hours} hours")


class PayRule(ABC):
    @abstractmethod
    def applies_to(self, worker):
        pass

    @abstractmethod
    def calculate(self, worker):
        pass


class HumanPayRule(PayRule):
    def applies_to(self, worker):
        return worker.worker_type == "human"

    def calculate(self, worker):
        return worker.hours * 20


class RobotPayRule(PayRule):
    def applies_to(self, worker):
        return worker.worker_type == "robot"

    def calculate(self, worker):
        return worker.hours * 15


class PayrollService(ABC):
    @abstractmethod
    def calculate(self, worker):
        pass


class RuleBasedPayrollService(PayrollService):
    def __init__(self, rules):
        self.rules = rules

    def calculate(self, worker):
        for rule in self.rules:
            if rule.applies_to(worker):
                return rule.calculate(worker)
        return 0


class WorkerRepository(ABC):
    @abstractmethod
    def save(self, worker):
        pass


class NotificationService(ABC):
    @abstractmethod
    def send_email(self, email, message):
        pass


class FileWorkerRepository(WorkerRepository):
    def __init__(self, path):
        self.path = path

    def save(self, worker):
        with open(self.path, "w", encoding="utf-8") as f:
            f.write(f"type={worker.worker_type},hours={worker.hours}\n")


class ConsoleNotificationService(NotificationService):
    def send_email(self, email, message):
        print(f"Sending to {email}: {message}")

class DisplayNotificationService(NotificationService):
    def send_email(self, email, message):
        print(f"Sending to {email}: {message}")


class WorkerCoordinator:
    def __init__(self, payroll_service, repository, notifier):
        self.payroll_service = payroll_service
        self.repository = repository
        self.notifier = notifier

    def run_shift(self, worker):
        worker.work()
        print("Pay:", self.payroll_service.calculate(worker))
        self.repository.save(worker)
        self.notifier.send_email("boss@company.com", "Shift finished")


class App:
    def __init__(self):
        rules = [HumanPayRule(), RobotPayRule()]
        payroll = RuleBasedPayrollService(rules)
        repository = FileWorkerRepository("worker.txt")
        notifier = DisplayNotificationService()
        self.coordinator = WorkerCoordinator(payroll, repository, notifier)

    def run(self):
        robot = RobotWorker(8)
        self.coordinator.run_shift(robot)


if __name__ == "__main__":
    App().run()
