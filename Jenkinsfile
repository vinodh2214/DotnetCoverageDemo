pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
        SONAR_SCANNER = "C:\\Users\\JALAGAM\\.dotnet\\tools\\dotnet-sonarscanner.exe"
    }

    stages {

        stage('Clean Workspace') {
            steps {
                deleteDir()
            }
        }

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
                      /d:sonar.cs.opencover.reportsPaths= ^
                      /d:sonar.cs.cobertura.reportsPaths=TestResults/coverage.cobertura.xml ^
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

        stage('Test + Coverage (Single File)') {
            steps {
                bat """
                dotnet test ^
                  --collect:"XPlat Code Coverage" ^
                  --results-directory TestResults ^
                  /p:CollectCoverage=true ^
                  /p:CoverletOutputFormat=cobertura ^
                  /p:CoverletOutput=TestResults/coverage.cobertura.xml
                """
            }
        }

        stage('Verify Coverage File') {
            steps {
                bat 'dir TestResults'
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
                cobertura(pattern: 'TestResults/coverage.cobertura.xml')
            ]
        }
    }
}
