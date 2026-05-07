pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
        GEMINI_API_KEY = credentials('GEMINI_API_KEY') 
        
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
                // Simplified because 'gemini' is now in the System PATH
                aiAgent(
                    agent: geminiCli(), 
                    model: 'gemini-1.5-pro',
                    prompt: "Review the C# code changes for logic bugs and best practices.",
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
                agent: geminiCli(),
                model: 'gemini-1.5-flash',
                prompt: "Identify the failure in the .NET build logs and propose a fix."
            )
        }
    }
}
