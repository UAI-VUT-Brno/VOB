from abc import ABC, abstractmethod


class Worker:
    def __init__(self, worker_type, hours):
        self.worker_type = worker_type
        self.hours = hours

    def work(self):
        print(f"{self.worker_type} working for {self.hours} hours")

    def eat(self):
        print(f"{self.worker_type} eating")

    def sleep(self):
        print(f"{self.worker_type} sleeping")


class RobotWorker(Worker):
    def eat(self):
        raise Exception("Robot does not eat")

    def sleep(self):
        raise Exception("Robot does not sleep")


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
    
class EctoPayRule(PayRule):
    def applies_to(self, worker):
        return worker.worker_type == "ectoplasm"

    def calculate(self, worker):
        return worker.hours * 10

class PayrollService:
    def __init__(self, rules):
        self.rules = rules

    def calculate(self, worker):
        for rule in self.rules:
            if rule.applies_to(worker):
                return rule.calculate(worker)
        return 0


class WorkerRepository:
    def save(self, worker, path):
        with open(path, "w", encoding="utf-8") as f:
            f.write(f"type={worker.worker_type},hours={worker.hours}\n")


class NotificationService:
    def send_email(self, email, message):
        print(f"Sending to {email}: {message}")


class WorkerCoordinator:
    def __init__(self):
        self.payroll = PayrollService([HumanPayRule(), RobotPayRule(), EctoPayRule()])
        self.repository = WorkerRepository()
        self.notifier = NotificationService()

    def run_shift(self, worker):
        worker.work()
        print("Pay:", self.payroll.calculate(worker))
        self.repository.save(worker, "worker.txt")
        self.notifier.send_email("boss@company.com", "Shift finished")
        worker.eat()


class App:
    def run(self):
        worker = RobotWorker("robot", 8)
        coordinator = WorkerCoordinator()
        coordinator.run_shift(worker)


if __name__ == "__main__":
    App().run()
