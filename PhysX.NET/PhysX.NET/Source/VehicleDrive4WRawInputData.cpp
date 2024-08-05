#include "stdafx.h"
#include "VehicleDrive4WRawInputData.h"

PhysX::VehicleDrive4WRawInputData::VehicleDrive4WRawInputData()
{
	_inputData = new PxVehicleDrive4WRawInputData();
}

PhysX::VehicleDrive4WRawInputData::~VehicleDrive4WRawInputData()
{
	this->!VehicleDrive4WRawInputData();
}

PhysX::VehicleDrive4WRawInputData::!VehicleDrive4WRawInputData()
{
	if (nullptr != _inputData) {
		delete _inputData;
		_inputData = nullptr;
	}
}

void PhysX::VehicleDrive4WRawInputData::SetDigitalAcceleration(bool keyPressed)
{
	_inputData->setDigitalAccel(keyPressed);
}

void PhysX::VehicleDrive4WRawInputData::SetDigitalSteerLeft(bool keyPressed)
{
	_inputData->setDigitalSteerLeft(keyPressed);
}

void PhysX::VehicleDrive4WRawInputData::SetDigitalSteerRight(bool keyPressed)
{
	_inputData->setDigitalSteerRight(keyPressed);
}

void PhysX::VehicleDrive4WRawInputData::SetDigitalBrake(bool keyPressed)
{
	_inputData->setDigitalBrake(keyPressed);
}

void PhysX::VehicleDrive4WRawInputData::SetDigitalHandbrake(bool keyPressed)
{
	_inputData->setDigitalHandbrake(keyPressed);
}

void PhysX::VehicleDrive4WRawInputData::SetAnalogAcceleration(float acceleration)
{
	_inputData->setAnalogAccel(acceleration);
}

void PhysX::VehicleDrive4WRawInputData::SetAnalogSteer(float steer)
{
	_inputData->setAnalogSteer(steer);
}

void PhysX::VehicleDrive4WRawInputData::SetAnalogBrake(float brake)
{
	_inputData->setAnalogBrake(brake);
}

void PhysX::VehicleDrive4WRawInputData::SetAnalogHandbrake(float handbrake)
{
	_inputData->setAnalogHandbrake(handbrake);
}

PxVehicleDrive4WRawInputData* PhysX::VehicleDrive4WRawInputData::UnmanagedPointer::get()
{
	return _inputData;
}