pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
        // Use the ID you gave your secret in Jenkins Credentials
        AI_CREDENTIALS_ID = 'GEMINI_API_KEY' 
        
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
                // FIXED SYNTAX: agentType is replaced by the specific handler symbol
                aiAgent(
                    handler: geminiCli(
                        credentialsId: "${AI_CREDENTIALS_ID}",
                        model: 'gemini-1.5-pro'
                    ),
                    prompt: "Review the C# changes. Identify security flaws and logic bugs. Provide feedback as inline-style comments for the logs.",
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
            // FIXED SYNTAX: Also updated the failure block
            aiAgent(
                handler: geminiCli(
                    credentialsId: "${AI_CREDENTIALS_ID}",
                    model: 'gemini-1.5-flash'
                ),
                prompt: "Analyze these .NET build logs and provide a fix: ${env.BUILD_URL}"
            )
        }
    }
}
