Feature: SchedulesDefiningFeature

This feature allows a manager to define and publish daily working schedules for employees within a company.
The feature ensures that employees' schedules are accessible after being published,
while restricting non-manager users from publishing schedules, enforcing access control.

@publishingSchedules
Scenario: Defining and publishing daily working schedules
	Given a manager assigned to a business unit exists
	And when manager defines working hours for employees for the upcoming month using POST /companies/{companyIdentifier}/schedules/
	When the manager publishes the defined schedule using POST /companies/{companyIdentifier}/schedules/{createdScheduleIdentifier}/publish
	Then all employees' daily schedules for the defined month are available via GET /companies/{companyIdentifier}/schedules/{month-year}
	And individual employee daily schedules for the defined month are available via GET /employee/{employeeId}/schedules/{dailyScheduleDate}

Scenario: Non-manager tries to publish a schedule
	Given a non-manager user tries to publish the schedule using POST /companies/{companyIdentifier}/schedules/{createdScheduleIdentifier}/publish
	Then the system returns an error indicating insufficient permissions