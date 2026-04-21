pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
        SONAR_SCANNER = "C:\\Users\\JALAGAM\\.dotnet\\tools\\dotnet-sonarscanner.exe"
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
                      /d:sonar.coverageReportPaths=**/coverage.cobertura.xml ^
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

        stage('Test + Coverage (XPlat)') {
            steps {
                bat """
                dotnet test ^
                  --collect:"XPlat Code Coverage" ^
                  --results-directory TestResults ^
                  --logger "trx"
                """
            }
        }

        stage('Debug Coverage File') {
            steps {
                bat 'dir /s coverage.cobertura.xml'
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
        always {
            // Test results
            junit '**/*.trx'

            // Jenkins Coverage Visualization
            recordCoverage tools: [
                cobertura(pattern: '**/coverage.cobertura.xml')
            ]
        }
    }
}
