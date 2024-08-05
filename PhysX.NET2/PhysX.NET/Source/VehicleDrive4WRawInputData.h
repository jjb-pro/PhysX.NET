#pragma once
#include "vehicle\PxVehicleUtilControl.h"

namespace PhysX {
	public ref class VehicleDrive4WRawInputData
	{
		public:
			VehicleDrive4WRawInputData();
			~VehicleDrive4WRawInputData();
			!VehicleDrive4WRawInputData();

		private:
			PxVehicleDrive4WRawInputData* _inputData;

		internal:
			property PxVehicleDrive4WRawInputData* UnmanagedPointer
			{
				PxVehicleDrive4WRawInputData* get();
			}

		public:
			void SetDigitalAcceleration(bool keyPressed);
			void SetDigitalSteerLeft(bool keyPressed);
			void SetDigitalSteerRight(bool keyPressed);
			void SetDigitalBrake(bool keyPressed);
			void SetDigitalHandbrake(bool keyPressed);
			void SetAnalogAcceleration(float acceleration);
			void SetAnalogSteer(float steer);
			void SetAnalogBrake(float brake);
			void SetAnalogHandbrake(float handbrake);
	};
}