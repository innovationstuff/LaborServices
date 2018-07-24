GO;
MERGE INTO Candidate  
USING [dbo].[Candidate_SqlType] 
ON Candidate_SqlType.Labor_VisaNumber = Candidate.Labor_VisaNumber
WHEN MATCHED THEN 
				UPDATE 
					   SET	Labor_VisaNumber 			 = Candidate_SqlType.Labor_VisaNumber,
							DateOfBirth					 = Candidate_SqlType.DateOfBirth,
							Profession					 = Candidate_SqlType.Profession,
							Country						 = Candidate_SqlType.Country,
							PassportNumber				 = Candidate_SqlType.PassportNumber,
							IssueDate					 = Candidate_SqlType.IssueDate,
							BlockVisaNo					 = Candidate_SqlType.BlockVisaNo,
							[Authorization]				 = Candidate_SqlType.[Authorization],
							ProjectCode					 = Candidate_SqlType.ProjectCode,
							ProjectName					 = Candidate_SqlType.ProjectName,
							FlightNumber				 = Candidate_SqlType.FlightNumber,
							AirCompany					 = Candidate_SqlType.AirCompany,
							DepartureTime				 = Candidate_SqlType.DepartureTime,
							ArrivalDate					 = Candidate_SqlType.ArrivalDate,
							ArrivalTime					 = Candidate_SqlType.ArrivalTime,
							ArrivalPlace				 = Candidate_SqlType.ArrivalPlace,
							BasicSalary					 = Candidate_SqlType.BasicSalary,
							Foodallowance				 = Candidate_SqlType.Foodallowance,
							HousingAllowance			 = Candidate_SqlType.HousingAllowance,
							OtherAllowance				 = Candidate_SqlType.OtherAllowance,
							TransportationAllowance 	 = Candidate_SqlType.TransportationAllowance,
							ExpectedArrivalDate			 = Candidate_SqlType.ExpectedArrivalDate,
						    [Name]						 = Candidate_SqlType.[Name]
WHEN NOT MATCHED THEN 
				INSERT 
					(
					Labor_VisaNumber 			 ,
					DateOfBirth					 ,
					Profession					 ,
					Country						 ,
					PassportNumber				 ,
					IssueDate					 ,
					BlockVisaNo					 ,
					[Authorization]				 ,
					ProjectCode					 ,
					ProjectName					 ,
					FlightNumber				 ,
					AirCompany					 ,
					DepartureTime				 ,
					ArrivalDate					 ,
					ArrivalTime					 ,
					ArrivalPlace				 ,
					BasicSalary					 ,
					Foodallowance				 ,
					HousingAllowance			 ,
					OtherAllowance				 ,
					TransportationAllowance 	 ,
					ExpectedArrivalDate			 ,
					[Name]					     
					)
				Values
					(
					Candidate_SqlType.Labor_VisaNumber,
					Candidate_SqlType.DateOfBirth,
					Candidate_SqlType.Profession,
					Candidate_SqlType.Country,
					Candidate_SqlType.PassportNumber,
					Candidate_SqlType.IssueDate,
					Candidate_SqlType.BlockVisaNo,
					Candidate_SqlType.[Authorization],
					Candidate_SqlType.ProjectCode,
					Candidate_SqlType.ProjectName,
					Candidate_SqlType.FlightNumber,
					Candidate_SqlType.AirCompany,
					Candidate_SqlType.DepartureTime,
					Candidate_SqlType.ArrivalDate,
					Candidate_SqlType.ArrivalTime,
					Candidate_SqlType.ArrivalPlace,
					Candidate_SqlType.BasicSalary,
					Candidate_SqlType.Foodallowance,
				    Candidate_SqlType.HousingAllowance,
					Candidate_SqlType.OtherAllowance,
					Candidate_SqlType.TransportationAllowance,
					Candidate_SqlType.ExpectedArrivalDate,
					Candidate_SqlType.[Name]
					)
WHEN NOT MATCHED BY SOURCE THEN 
				Update
					Set IsDeleted = 1,
						DeletedOn = GetDate()
;