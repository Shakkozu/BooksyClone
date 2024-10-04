Feature: Availability

A short summary of the feature

@reservationOfThetimeslot
Scenario: Reservation of the timeslot by a customer
	Given there's day daily schedule of the worker without reservations
	When customer tries to book a reservation outside working hours scope
	Then error is throwed which states that reservation can be created only within working hours
	When customer tries to book a reservation within a working hours
	Then reservation is created successfully
	When another customer tries to book a reservation with different timeslot
	Then reservation is created successfully
	When another customer tries to book a reservation with conflicting timeslot
	Then error is throwed due to already reservaed timeslot


@lockingAndReleasingASchedule
Scenario: Locking and releasing a schedule by an employee
    Given an employee's daily schedule for the day 30-09-2024 exists with some reservations
    When I fetch the employee's daily schedule using GET /employee/{employeeId}/schedules/30-09-2024
    Then the response shows available timeslots and reserved timeslots

    When the employee creates a blockade for a timeslot using POST /employee/{employeeId}/schedules/blockades
    Then a blockade is created
    And fetching the daily schedule returns the existing blockade alongside reservations, showing any conflicts

    When a customer attempts to book a reservation within the blocked timeslot using POST /reservations
    Then the response returns an error with information that the selected timeslot is unavailable

    When the employee releases the blockade using DELETE /employee/{employeeId}/schedules/blockades/{blockadeIdentifier}
    Then the blockade is removed
    And fetching the daily schedule shows no blockades and no conflicts

    When the customer attempts to book a reservation for the timeslot that was previously blocked using POST /reservations
    Then the reservation is successfully created
    And the customer can view their reservation under GET /reservations/{customerId}
