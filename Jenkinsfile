pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
        GEMINI_API_KEY = credentials('GEMINI_API_KEY') 
        
        // Use the .cmd path found in your 'where' command
        GEMINI_BIN = "C:\\Users\\JALAGAM\\AppData\\Roaming\\npm\\gemini.cmd" 
        
        SONAR_SCANNER = "C:\\Users\\JALAGAM\\.dotnet\\tools\\dotnet-sonarscanner.exe"
        DOTNET_COVERAGE = "C:\\Users\\JALAGAM\\.dotnet\\tools\\dotnet-coverage.exe"
        REPORT_GENERATOR = "C:\\Users\\JALAGAM\\.dotnet\\tools\\reportgenerator.exe"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('AI Code Review') {
            steps {
                aiAgent(
                    agent: geminiCli(customBin: "${GEMINI_BIN}"), 
                    model: 'gemini-1.5-pro',
                    prompt: "Review the C# changes in this .NET project. Focus on performance and NullReferenceExceptions.",
                    yoloMode: true
                )
            }
        }

        stage('Sonar Begin') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat """
                    "%SONAR_SCANNER%" begin ^
                      /k:"dotnet-project" ^
                      /d:sonar.login=%SONAR_TOKEN% ^
                      /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml ^
                      /d:sonar.exclusions=**/Program.cs,**/Startup.cs,**/*.g.cs,**/bin/**,**/obj/** ^
                      /d:sonar.tests=**/*.Tests
                    """
                }
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build'
            }
        }

        // ... Keep your Test and Report stages as they are ...

        stage('Sonar End') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat """
                    "%SONAR_SCANNER%" end ^
                      /d:sonar.login=%SONAR_TOKEN%
                    """
                }
            }
        }
    }

    post {
        failure {
            aiAgent(
                agent: geminiCli(customBin: "${GEMINI_BIN}"),
                model: 'gemini-1.5-flash',
                prompt: "Identify why the .NET build failed from the logs and suggest a code fix."
            )
        }
    }
}
