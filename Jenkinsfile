pipeline {
    agent any

    environment {
        SONAR_TOKEN = credentials('sonar')
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore .NET Tools (Manifest)') {
            steps {
                bat 'dotnet tool restore'
            }
        }

        stage('Sonar Begin') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat """
                    dotnet tool run dotnet-sonarscanner begin ^
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
                bat 'dotnet restore'
                bat 'dotnet build --no-restore'
            }
        }

        stage('Test + Coverage') {
            steps {
                bat """
                dotnet tool run dotnet-coverage collect ^
                  "dotnet test --no-build --logger \\"junit;LogFilePath=test-results.xml\\"" ^
                  -f xml ^
                  -o coverage.xml
                """
            }
        }

        stage('Generate HTML Coverage Report') {
            steps {
                bat """
                dotnet tool run reportgenerator ^
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
                    dotnet tool run dotnet-sonarscanner end ^
                      /d:sonar.login=%SONAR_TOKEN%
                    """
                }
            }
        }
    }

    post {
        always {
            // Publish test results in Jenkins UI
            junit '**/test-results.xml'

            // Publish coverage in Jenkins UI (VS Coverage XML format)
            publishCoverage adapters: [
                visualStudioAdapter('coverage.xml')
            ]
        }
    }
}
