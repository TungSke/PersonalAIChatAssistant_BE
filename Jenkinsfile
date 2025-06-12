pipeline {
    agent any

    environment {
        DOCKERHUB_CREDENTIALS = credentials('dockerhub-creds') // ID của Docker Hub credentials trong Jenkins
        IMAGE_NAME = 'tungtrinh2003/devop:latest'
    }

    stages {

        stage('Checkout Code') {
            steps {
                checkout scm
            }
        }

        // stage('Login to Docker Hub') {
        //     steps {
        //         script {
        //             sh "echo ${DOCKERHUB_CREDENTIALS_PSW} | docker login -u ${DOCKERHUB_CREDENTIALS_USR} --password-stdin"
        //         }
        //     }
        // }
        

        stage('Build Docker Image') {
            steps {
                sh "docker build -t ${IMAGE_NAME} ."
            }
        }

        stage('Push Docker Image') {
            steps {
                sh "docker push ${IMAGE_NAME}"
            }
            steps {
                sh "docker run -d -p 8080:8080 ${IMAGE_NAME}"
            }
        }
    }

    post {
        always {
            sh 'docker logout'
        }
    }
}