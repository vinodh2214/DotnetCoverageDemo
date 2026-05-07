pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
        // Your Gemini Key ID from Jenkins Credentials
        AI_CRED_ID = 'GEMINI_API_KEY' 
        
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
                // Use 'agent' to specify the handler and 'model' for the version
                aiAgent(
                    agent: geminiCli(credentialsId: "${AI_CRED_ID}"),
                    model: 'gemini-1.5-pro',
                    prompt: "Review the C# changes in this .NET project. Focus on best practices and potential logic bugs.",
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

        stage('Test + Coverage') {
            steps {
                bat """
                "%DOTNET_COVERAGE%" collect "dotnet test" -f xml -o coverage.xml
                """
            }
        }

        stage('Generate HTML Coverage Report') {
            steps {
                bat """
                "%REPORT_GENERATOR%" ^
                  -reports:coverage.xml ^
                  -targetdir:coverage-report ^
                  -reporttypes:Html ^
                  -assemblyfilters:+* ^
                  -classfilters:+* ^
                  -filefilters:-*Program.cs;-*Startup.cs;-*.g.cs ^
                  -verbosity:Info
                """
            }
        }

        stage('Publish Coverage (HTML)') {
            steps {
                archiveArtifacts artifacts: 'coverage-report/**', fingerprint: true
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
                agent: geminiCli(credentialsId: "${AI_CRED_ID}"),
                model: 'gemini-1.5-flash',
                prompt: "Analyze the .NET build logs and provide a fix for the failure."
            )
        }
    }
}
