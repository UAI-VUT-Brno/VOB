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

class EctoplasmWorker(Worker):
    def eat(self):
        pass

    def sleep(self):
        pass
    
class PayrollService:
    def calculate(self, worker):
        if worker.worker_type == "human":
            return worker.hours * 20
        if worker.worker_type == "robot":
            return worker.hours * 15
        if worker.worker_type == "ectoplasm":
            return worker.hours * 10
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
        self.payroll = PayrollService()
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
