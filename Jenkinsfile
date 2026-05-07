pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
        // The plugin will automatically look for this ENV variable
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
                // Simplified syntax: 'agent' just points to the type
                aiAgent(
                    agent: geminiCli(), 
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
                agent: geminiCli(),
                model: 'gemini-1.5-flash',
                prompt: "Identify the failure in these .NET build logs and propose a fix."
            )
        }
    }
}
