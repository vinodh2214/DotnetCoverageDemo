pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
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

        stage('Sonar Begin') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat """
                    "%SONAR_SCANNER%" begin ^
                      /k:"dotnet-project" ^
                      /d:sonar.login=%SONAR_TOKEN% ^
                      /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml
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

        stage('Convert Coverage Report') {
            steps {
                bat """
                "%REPORT_GENERATOR%" ^
                  -reports:coverage.xml ^
                  -targetdir:coverage-report ^
                  -reporttypes:Html;Cobertura
                """
            }
        }

        stage('Publish Coverage') {
    steps {
        recordCoverage tools: [
            cobertura(pattern: 'coverage-report/Cobertura.xml')
        ]
    }
}

        stage('Archive HTML Report') {
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
