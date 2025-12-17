using Microsoft.Extensions.Options;
using RedisCacheService.ConfigOptions;
using RedisCacheService.Models;

namespace RedisCacheService.TestCommon.MockData
{
  public class BaldoMock
  {
    public static IOptions<BaldoOptions> GetBaldoOptions => Options.Create(new BaldoOptions
    {
      ApiUrl = new Uri("https://api.com/int-test/user/v1/"),
      ApiClientId = "test",
    });
    public static IOptions<RedisCacheOptions> GetRedisCacheOptions => Options.Create(new RedisCacheOptions
    {
      CacheExpirationInHours = 8
    });

    public static IOptions<HttpClientSetting> GetHttpClientSetting => Options.Create(new HttpClientSetting
    {
      HttpClientName = "Baldo",
      RequestTimeout = 5
    });

    public static User CreateUserAuthorizationsResponse(
      string userId = "B54543",
      string firstName = "Alice",
      string LastName = "Bob",
      string principalOrgId = "1234XXYY",
      string orgName = "VTC Retail",
      bool IsPilotUser = true)
    {
      return new User
      {
        UserId = userId,
        FirstName = firstName,
        LastName = LastName,
        PrincipalOrganizationId = principalOrgId,
        OrganizationName = orgName
      };
    }

    public static string CreateCurrentUserResponse()
    {
      return $@"{{
          ""firstName"": ""Alice"",
          ""lastName"": ""Bob"",
          ""email"": ""bob.alice.@volvo.com"",
          ""languageCode"": ""en-US"",
          ""secondaryLanguageCode"": ""en-GB"",
          ""mainAddress"": {{
              ""addressLine1"": ""Assar Gabrielssons Göteborg Sweden"",
              ""postalCode"": ""405 08"",
              ""city"": ""Göteborg"",
              ""countryCode"": ""SE""
          }},
          ""contactInfo"": {{
              ""phone"": ""0000""
          }},
          ""userId"": ""B54543"",
          ""userOrganizations"": [
              ""1234XXYY""
          ]
      }}";
    }

    public static string CreateCurrentUserResponseWithEmptyUserOrganizations()
    {
      return $@"{{
          ""firstName"": ""Alice"",
          ""lastName"": ""Bob"",
          ""email"": ""bob.alice.@volvo.com"",
          ""languageCode"": ""en-US"",
          ""secondaryLanguageCode"": ""en-GB"",
          ""mainAddress"": {{
              ""addressLine1"": ""Assar Gabrielssons Göteborg Sweden"",
              ""postalCode"": ""405 08"",
              ""city"": ""Göteborg"",
              ""countryCode"": ""SE""
          }},
          ""contactInfo"": {{
              ""phone"": ""0000""
          }},
          ""userId"": ""B54543"",
          ""userOrganizations"": []
      }}";
    }

    public static string CreateAuthorizationResponse()
    {
      return $@"{{
          ""organizationId"": ""1234XXYY"",
          ""baldoOrganizationId"": ""LM500128"",
          ""baBrands"": [
		          {{
			          ""businessArea"": {{
				          ""id"": ""1"",
				          ""description"": ""Trucks""
			          }},
			          ""brand"": {{
				          ""id"": ""100"",
				          ""description"": ""Volvo""
			          }}
		          }},
		          {{
			          ""businessArea"": {{
				          ""id"": ""1"",
				          ""description"": ""Trucks""
			          }},
			          ""brand"": {{
				          ""id"": ""120"",
				          ""description"": ""Renault""
			          }}
		          }},
		          {{
			          ""businessArea"": {{
				          ""id"": ""1"",
				          ""description"": ""Trucks""
			          }},
			          ""brand"": {{
				          ""id"": ""110"",
				          ""description"": ""Mack""
			          }}
		          }},
		          {{
			          ""businessArea"": {{
				          ""id"": ""2"",
				          ""description"": ""Bus""
			          }},
			          ""brand"": {{
				          ""id"": ""100"",
				          ""description"": ""Volvo""
			          }}
		          }},
		          {{
			          ""businessArea"": {{
				          ""id"": ""1"",
				          ""description"": ""Trucks""
			          }},
			          ""brand"": {{
				          ""id"": ""150"",
				          ""description"": ""Eicher""
			          }}
		          }},
              {{
                ""businessArea"": {{
                  ""id"": ""3"",
                  ""description"": ""Construction Equipment""
                }},
                ""brand"": {{
                  ""id"": ""130"",
                  ""description"": ""VCE""
                }}
              }}
	        ],
          ""authorizations"": {{
              ""Application"": [{{
                      ""Id"": {{
                          ""_text"": ""114458""
                      }},
                      ""Description"": {{
                          ""_text"": ""OneView""
                      }},              
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""Basic_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Basic User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""7281a76c-d4b1-41e4-b429-2d49f2c4dfe7""
                              }},
                              ""Description"": {{
                                  ""_text"": ""226678_USER""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }}
                          }}
                      ]
                  }}, {{
                      ""Id"": {{
                          ""_text"": ""99113""
                      }},
                      ""Description"": {{
                          ""_text"": ""CMS""
                      }},
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""READER""
                              }},
                              ""Description"": {{
                                  ""_text"": ""Reader""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""WRITER""
                              }},
                              ""Description"": {{
                                  ""_text"": ""Writer""
                              }}
                          }}
                      ]
                  }}, {{
                      ""Id"": {{
                          ""_text"": ""GSP""
                      }},
                      ""Description"": {{
                          ""_text"": ""GSP""
                      }},
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""c2180b9d-a7ce-4ecb-af0a-28544081867b""
                              }},
                              ""Description"": {{
                                  ""_text"": ""User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""Triage_Basic""
                              }},
                              ""Description"": {{
                                  ""_text"": ""Triage_Basic""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""Internal_Preview""
                              }},
                              ""Description"": {{
                                  ""_text"": ""Internal_Preview""
                              }}
                          }}
                      ]
                  }}, {{
                      ""Id"": {{
                          ""_text"": ""TRIAGE""
                      }},
                      ""Description"": {{
                          ""_text"": ""REF_TRIAGE""
                      }},
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""TR_LEVEL_1""
                              }},
                              ""Description"": {{
                                  ""_text"": ""TR LEVEL 1""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""37bb8aa8-bb59-4d87-a996-1676b1bcc988""
                              }},
                              ""Description"": {{
                                  ""_text"": ""TR_USER""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""TR_LEVEL_2""
                              }},
                              ""Description"": {{
                                  ""_text"": ""TR_LEVEL_2""
                              }}
                          }}
                      ]
                  }}, {{
                      ""Id"": {{
                          ""_text"": ""VBC""
                      }},
                      ""Description"": {{
                          ""_text"": ""VBC""
                      }},
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""VBC_Workshop_Standard_User""
                              }},
                              ""Description"": {{
                                  ""_text"": ""VBC Workshop Standard User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""VBC_Workshop_Diagnostic_User""
                              }},
                              ""Description"": {{
                                  ""_text"": ""VBC Workshop Diagnostic User""
                              }}
                          }}
                      ]
                  }}
              ],
              ""Role"": [{{
                      ""Name"": {{
                          ""_text"": ""Dealer Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""External Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""Importer Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""Internal Info""
                      }}
                  }}
              ]
          }}
      }}";
    }

    public static string CreateAuthorizationWithWorkshopResponse()
    {
      return $@"{{
          ""organizationId"": ""1234XXYY"",
          ""baldoOrganizationId"": ""LM500128"",
          ""authorizations"": {{
              ""Application"": [{{
                      ""Id"": {{
                          ""_text"": ""114458""
                      }},
                      ""Description"": {{
                          ""_text"": ""OneView""
                      }},              
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""Basic_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Basic User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""7281a76c-d4b1-41e4-b429-2d49f2c4dfe7""
                              }},
                              ""Description"": {{
                                  ""_text"": ""226678_USER""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""Component_alert_view_workshop_level""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Component alert view – workshop level""
                              }}
                          }}
                      ]
                  }}
              ]
          }}
      }}";
    }

    public static string CreateAuthorizationWithMainDealerResponse()
    {
      return $@"{{
          ""organizationId"": ""1234XXYY"",
          ""baldoOrganizationId"": ""LM500128"",
          ""authorizations"": {{
              ""Application"": [{{
                      ""Id"": {{
                          ""_text"": ""114458""
                      }},
                      ""Description"": {{
                          ""_text"": ""OneView""
                      }},              
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""Basic_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Basic User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""7281a76c-d4b1-41e4-b429-2d49f2c4dfe7""
                              }},
                              ""Description"": {{
                                  ""_text"": ""226678_USER""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""Component_alert_view_main dealer_level""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Component alert view – main dealer level""
                              }}
                          }}
                      ]
                  }}
              ]
          }}
      }}";
    }

    public static string CreateAuthorizationWithMarketResponse()
    {
      return $@"{{
          ""organizationId"": ""1234XXYY"",
          ""baldoOrganizationId"": ""LM500128"",
          ""authorizations"": {{
              ""Application"": [{{
                      ""Id"": {{
                          ""_text"": ""114458""
                      }},
                      ""Description"": {{
                          ""_text"": ""OneView""
                      }},              
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""Basic_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Basic User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""7281a76c-d4b1-41e4-b429-2d49f2c4dfe7""
                              }},
                              ""Description"": {{
                                  ""_text"": ""226678_USER""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""Component_alert_view_market_level""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Component alert view – market level""
                              }}
                          }}
                      ]
                  }}
              ]
          }}
      }}";
    }

    public static string CreateAuthorizationWithAllMarketResponse()
    {
      return $@"{{
          ""organizationId"": ""1234XXYY"",
          ""baldoOrganizationId"": ""LM500128"",
          ""authorizations"": {{
              ""Application"": [{{
                      ""Id"": {{
                          ""_text"": ""114458""
                      }},
                      ""Description"": {{
                          ""_text"": ""OneView""
                      }},              
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""Basic_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Basic User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""7281a76c-d4b1-41e4-b429-2d49f2c4dfe7""
                              }},
                              ""Description"": {{
                                  ""_text"": ""226678_USER""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""OneView_Pilot_User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""Component_alert_view_all_markets""
                              }},                        
                              ""Description"": {{
                                  ""_text"": ""Component alert view – all markets""
                              }}
                          }}
                      ]
                  }}
              ]
          }}
      }}";
    }

    public static string CreateAuthorizationResponseWithNoApplications()
    {
      return $@"{{
          ""organizationId"": ""1234XXYY"",
          ""baldoOrganizationId"": ""LM500128"",
          ""authorizations"": {{
              ""Role"": [{{
                      ""Name"": {{
                          ""_text"": ""Dealer Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""External Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""Importer Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""Internal Info""
                      }}
                  }}
              ]
          }}
      }}";
    }

    public static string CreateAuthorizationResponseWithEmptyApplicationsList()
    {
      return $@"{{
          ""organizationId"": ""1234XXYY"",
          ""baldoOrganizationId"": ""LM500128"",
          ""authorizations"": {{
              ""Application"": [],
              ""Role"": [{{
                      ""Name"": {{
                          ""_text"": ""Dealer Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""External Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""Importer Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""Internal Info""
                      }}
                  }}
              ]
          }}
      }}";
    }

    public static string CreateAuthorizationResponseWithNoOneviewInApplicationsList()
    {
      return $@"{{
          ""organizationId"": ""1234XXYY"",
          ""baldoOrganizationId"": ""LM500128"",
          ""authorizations"": {{
              ""Application"": [{{
                      ""Id"": {{
                          ""_text"": ""99113""
                      }},
                      ""Description"": {{
                          ""_text"": ""CMS""
                      }},
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""READER""
                              }},
                              ""Description"": {{
                                  ""_text"": ""Reader""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""WRITER""
                              }},
                              ""Description"": {{
                                  ""_text"": ""Writer""
                              }}
                          }}
                      ]
                  }}, {{
                      ""Id"": {{
                          ""_text"": ""GSP""
                      }},
                      ""Description"": {{
                          ""_text"": ""GSP""
                      }},
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""c2180b9d-a7ce-4ecb-af0a-28544081867b""
                              }},
                              ""Description"": {{
                                  ""_text"": ""User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""Triage_Basic""
                              }},
                              ""Description"": {{
                                  ""_text"": ""Triage_Basic""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""Internal_Preview""
                              }},
                              ""Description"": {{
                                  ""_text"": ""Internal_Preview""
                              }}
                          }}
                      ]
                  }}, {{
                      ""Id"": {{
                          ""_text"": ""TRIAGE""
                      }},
                      ""Description"": {{
                          ""_text"": ""REF_TRIAGE""
                      }},
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""TR_LEVEL_1""
                              }},
                              ""Description"": {{
                                  ""_text"": ""TR LEVEL 1""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""37bb8aa8-bb59-4d87-a996-1676b1bcc988""
                              }},
                              ""Description"": {{
                                  ""_text"": ""TR_USER""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""TR_LEVEL_2""
                              }},
                              ""Description"": {{
                                  ""_text"": ""TR_LEVEL_2""
                              }}
                          }}
                      ]
                  }}, {{
                      ""Id"": {{
                          ""_text"": ""VBC""
                      }},
                      ""Description"": {{
                          ""_text"": ""VBC""
                      }},
                      ""Category"": [{{
                              ""Name"": {{
                                  ""_text"": ""VBC_Workshop_Standard_User""
                              }},
                              ""Description"": {{
                                  ""_text"": ""VBC Workshop Standard User""
                              }}
                          }}, {{
                              ""Name"": {{
                                  ""_text"": ""VBC_Workshop_Diagnostic_User""
                              }},
                              ""Description"": {{
                                  ""_text"": ""VBC Workshop Diagnostic User""
                              }}
                          }}
                      ]
                  }}
              ],
              ""Role"": [{{
                      ""Name"": {{
                          ""_text"": ""Dealer Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""External Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""Importer Info""
                      }}
                  }}, {{
                      ""Name"": {{
                          ""_text"": ""Internal Info""
                      }}
                  }}
              ]
          }}
      }}";
    }

    public static string CreateUserOrganizationResponse(
      string orgName = "VTC Retail",
      bool isPrincipal = true)
    {
      return $@"[{{
          ""organizationId"": ""1234XXYY"",
          ""name"": ""{orgName}"",
          ""principal"": ""{isPrincipal}""
      }}]";
    }
  }
}
