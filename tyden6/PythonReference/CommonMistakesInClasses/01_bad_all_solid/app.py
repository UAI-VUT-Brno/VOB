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

    def calculate_pay(self):
        if self.worker_type == "human":
            return self.hours * 20
        if self.worker_type == "robot":
            return self.hours * 15
        return 0

    def save_to_file(self, path):
        with open(path, "w", encoding="utf-8") as f:
            f.write(f"type={self.worker_type},hours={self.hours}\n")

    def send_email(self, email):
        print(f"Sending worker report to {email}")


class RobotWorker(Worker):
    def eat(self):
        raise Exception("Robot does not eat")

    def sleep(self):
        raise Exception("Robot does not sleep")


class App:
    def run(self):
        worker = RobotWorker("robot", 8)
        worker.work()
        print("Pay:", worker.calculate_pay())
        worker.save_to_file("worker.txt")
        worker.send_email("boss@company.com")
        worker.eat()


if __name__ == "__main__":
    App().run()
