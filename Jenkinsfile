pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
        SONAR_SCANNER = "C:\\Users\\JALAGAM\\.dotnet\\tools\\dotnet-sonarscanner.exe"
        DOTNET_COVERAGE = "C:\\Users\\JALAGAM\\.dotnet\\tools\\dotnet-coverage.exe"
        REPORT_GENERATOR = "C:\\Users\\JALAGAM\\.dotnet\\tools\\reportgenerator.exe"
    }

    stages {

        stage('Checkout main') {
            steps {
                checkout scm
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
}
