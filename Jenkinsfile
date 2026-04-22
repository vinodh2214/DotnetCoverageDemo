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
                      /d:sonar.cs.opencover.reportsPaths=**/coverage.cobertura.xml ^
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

        stage('Test + XPlat Coverage') {
            steps {
                bat """
                dotnet test ^
                  --collect:"XPlat Code Coverage" ^
                  --results-directory TestResults
                """
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
            recordCoverage tools: [
                cobertura(pattern: '**/coverage.cobertura.xml')
            ]
        }
    }
}
}
